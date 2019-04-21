using CodingTrainer.CodingTrainerModels;
using CodingTrainer.CSharpRunner.Assessment.Methods;
using CodingTrainer.CSharpRunner.CodeHost;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CodingTrainer.CSharpRunner.Assessment
{
    public class AssessmentRunner : IAssessmentRunner
    {
        public event ConsoleWriteEventHandler ConsoleWrite;

        private ICodeRunner _runner;
        private readonly string _code;
        private LazyAsync<CompiledCode> _compiledCode;
        private CompilationWithSource _compilation;
        private IEnumerable<Diagnostic> _diags;

        static AssessmentRunner()
        {
            try
            {
                var type = GetExpressionRunnerPredefinedTypes(out var predefinedTypesField, out var predefinedTypes);
                var newTypes = ExtractEnumTypes();
                predefinedTypes = MergeTypes(predefinedTypes, newTypes);
                SetPredefinedTypesFieldValue(predefinedTypesField, predefinedTypes, type);
            }
            catch { } // Exceptions must not prevent the _code from continuing - they will show later as exceptions when trying to use Dynamic Linq
        }

        private static Type[] ExtractEnumTypes()
        {
            // Add types to Dynamic Linq which might be used by assessment queries.
            // This has to happen once, before assessments run - doing it in the
            // static constructor should do the trick

            // EnumHelper is protected within AssessmentMethodBase
            Type assessmentMethodBase = typeof(AssessmentMethodBase);
            Type enumHelper = assessmentMethodBase.GetNestedType("EnumHelper`2", BindingFlags.NonPublic);

            Type[] newTypes =
            {
                enumHelper.MakeGenericType(typeof(Microsoft.CodeAnalysis.CSharp.SyntaxKind), typeof(ushort)),
                typeof(SyntaxToken),
                typeof(SyntaxNode)
            };
            return newTypes;
        }

        public AssessmentRunner(ICodeRunner runner, string code)
        {
            _runner = runner;
            _code = code;
        }

        public async Task<bool> RunAssessmentsAsync(IEnumerable<AssessmentGroup> assessmentGroups)
        {
            if (!await PrepareForCompiling()) return false;

            var result = await RunTests(assessmentGroups);
            if (result)
            {
                WriteToConsole("Congratulations!");
            }

            return result;
        }


        private async Task<bool> PrepareForCompiling()
        {
            try
            {
                _compilation = await _runner.GetCompilationAsync(_code);
                var tree = _compilation.CompilationObject.SyntaxTrees.Single();
                _diags = _compilation.CompilationObject.GetSemanticModel(tree).GetDiagnostics();

                _compiledCode = new LazyAsync<CompiledCode>
                    (() => _runner.EmitFromCompilationAsync(_compilation));
            }
            catch (Exception e) when (!(e is CompilationErrorException))
            {
                WriteToConsole("Something went wrong with the _compilation\r\n");
                WriteToConsole("The error message is:\r\n");
                WriteToConsole("  " + e.Message);
                return false;
            }

            return true;
        }

        private async Task<bool> RunTests(IEnumerable<AssessmentGroup> assessmentGroups)
        {
            var result = true;
            dynamic assessmentBag = new System.Dynamic.ExpandoObject();

            foreach (var assessmentGroup in assessmentGroups)
            {
                if (assessmentGroup.ShowAutoMessageOnStart)
                    WriteToConsole($"Checking {assessmentGroup.Title}\r\n");

                bool isGroupPassed = await RunAssessments(assessmentGroup, assessmentBag);

                if (isGroupPassed)
                {
                    if (assessmentGroup.ShowAutoMessageOnPass)
                        WriteToConsole(assessmentGroup.Title + " passed\r\n");
                }
                else
                {
                    result = false;

                    if (assessmentGroup.ShowAutoMessageOnFail)
                        WriteToConsole(assessmentGroup.Title + " failed\r\n");

                    if (assessmentGroup.EndAssessmentsOnFail) break;
                }
            } // End of assessment group loop

            return result;
        }

        private async Task<bool> RunAssessments(AssessmentGroup assessmentGroup, dynamic assessmentBag)
        {
            var groupResult = true;
            foreach (var assessmentBase in assessmentGroup.OrderedAssessments)
            {
                var assessment = (AssessmentMethodBase) assessmentBase;

                if (assessment.ShowAutoMessageOnStart)
                    WriteToConsole($"Checking {assessment.Title}\r\n");

                var thisResult = await RunAssessmentAsync(assessment, assessmentBag);

                if (thisResult)
                {
                    if (assessment.ShowAutoMessageOnPass)
                        WriteToConsole(assessment.Title + " passed\r\n");
                }
                else
                {
                    groupResult = false;

                    if (assessment.ShowAutoMessageOnFail)
                        WriteToConsole(assessment.Title + " failed\r\n");

                    if (assessment.EndAssessmentGroupOnFail) break;
                }
            }

            return groupResult;
        }

        private static Type GetExpressionRunnerPredefinedTypes(out FieldInfo predefinedTypesField, out Type[] predefinedTypes)
        {
            // Private type, hence we can't simply use typeof
            Type type = typeof(System.Linq.Dynamic.DynamicQueryable).Assembly.GetType("System.Linq.Dynamic.ExpressionParser");
            predefinedTypesField = type.GetField("predefinedTypes", BindingFlags.Static | BindingFlags.NonPublic);

            predefinedTypes = (Type[])predefinedTypesField.GetValue(null);
            return type;
        }

        private static Type[] MergeTypes(Type[] predefinedTypes, Type[] newTypes)
        {
            int originalLength = predefinedTypes.Length;
            Array.Resize(ref predefinedTypes, predefinedTypes.Length + newTypes.Length);
            predefinedTypes[predefinedTypes.Length - 1] = typeof(SyntaxToken);
            Array.Copy(newTypes, 0, predefinedTypes, originalLength, newTypes.Length);
            return predefinedTypes;
        }

        private static void SetPredefinedTypesFieldValue(FieldInfo predefinedTypesField, Type[] predefinedTypes, Type type)
        {
            predefinedTypesField.SetValue(null, predefinedTypes);
            predefinedTypesField = type.GetField("keywords", BindingFlags.Static | BindingFlags.NonPublic);
            predefinedTypesField.SetValue(null, null);
        }

        private async Task<bool> RunAssessmentAsync(AssessmentMethodBase assessment, dynamic assessmentBag)
        {
            assessment.AssessmentBag = assessmentBag;
            assessment.ConsoleWrite += OnConsoleWrite;
            if (assessment is AssessmentByInspectionBase inspectionAssessment)
            {
                inspectionAssessment.Compilation = _compilation;
            }
            else if (assessment is AssessmentByRunningBase runningAssessment)
            {
                if (_diags.Any(d => d.Severity == DiagnosticSeverity.Error))
                {
                    WriteToConsole("Unable to run your _code because it has compiler errors\r\n");
                    return false;
                }

                runningAssessment.CodeRunner = _runner;
                try
                {
                    runningAssessment.CompiledCode = await _compiledCode.Value;
                }
                catch (Exception e) when (!(e is CompilationErrorException))
                {
                    WriteToConsole("Something went wrong with the _compilation\r\n");
                    WriteToConsole("The error message is:\r\n");
                    WriteToConsole("  " + e.Message);
                    return false;
                }
            }

            return await assessment.AssessAsync();

        }

        private void WriteToConsole(string message)
        {
            ConsoleWrite?.Invoke(this, new ConsoleWriteEventArgs(message));
        }
        private void OnConsoleWrite(object sender, ConsoleWriteEventArgs e)
        {
            ConsoleWrite?.Invoke(this, e);
        }

        private class LazyAsync<T> : Lazy<Task<T>>
        {
            public LazyAsync(Func<Task<T>> taskFunc)
                : base(() => Task.Factory.StartNew(taskFunc).Unwrap())
            { }
        }
    }
}
