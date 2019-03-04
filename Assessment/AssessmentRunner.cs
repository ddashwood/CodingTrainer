using CodingTrainer.CodingTrainerModels;
using CodingTrainer.CSharpRunner.Assessment.Methods;
using CodingTrainer.CSharpRunner.CodeHost;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Scripting;
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
    public class AssessmentRunner
    {
        public event ConsoleWriteEventHandler ConsoleWrite;
        private ICodeRunner runner;
        private readonly string code;
        private LazyAsync<CompiledCode> compiledCode;
        private CompilationWithSource compilation;
        private IEnumerable<Diagnostic> diags;

        static AssessmentRunner()
        {
            try
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

                // Private type, hence we can't simply use typeof
                Type type = typeof(System.Linq.Dynamic.DynamicQueryable).Assembly.GetType("System.Linq.Dynamic.ExpressionParser");
                FieldInfo field = type.GetField("predefinedTypes", BindingFlags.Static | BindingFlags.NonPublic);

                Type[] predefinedTypes = (Type[])field.GetValue(null);

                int originalLength = predefinedTypes.Length;
                Array.Resize(ref predefinedTypes, predefinedTypes.Length + newTypes.Length);
                predefinedTypes[predefinedTypes.Length - 1] = typeof(SyntaxToken);
                Array.Copy(newTypes, 0, predefinedTypes, originalLength, newTypes.Length);

                field.SetValue(null, predefinedTypes);

                field = type.GetField("keywords", BindingFlags.Static | BindingFlags.NonPublic);
                field.SetValue(null, null);
            }
            catch { } // Exceptions must not prevent the code from continuing - they will show later as exceptions when trying to use Dynamic Linq
        }


        public AssessmentRunner(ICodeRunner runner, string code)
        {
            this.runner = runner;
            this.code = code;
        }

        public async Task<bool> RunAssessmentsAsync(IEnumerable<AssessmentGroup> assessmentGroups)
        {
            // Prepare to compile the code

            try
            {
                compilation = await runner.GetCompilationAsync(code);
                var tree = compilation.CompilationObject.SyntaxTrees.Single();
                diags = compilation.CompilationObject.GetSemanticModel(tree).GetDiagnostics();

                compiledCode = new LazyAsync<CompiledCode>
                        (() => runner.EmitFromCompilationAsync(compilation));
            }
            catch (Exception e) when (!(e is CompilationErrorException))
            {
                WriteToConsole("Something went wrong with the compilation\r\n");
                WriteToConsole("The error message is:\r\n");
                WriteToConsole("  " + e.Message);
                return false;
            }

            // Now run each of the tests

            var result = true;
            dynamic assessmentBag = new System.Dynamic.ExpandoObject();

            foreach (var assessmentGroup in assessmentGroups)
            {
                if (assessmentGroup.ShowAutoMessageOnStart)
                    WriteToConsole($"Checking {assessmentGroup.Title}\r\n");

                var groupResult = true;
                foreach (var assessmentBase in assessmentGroup.OrderedAssessments)
                {
                    var assessment = (AssessmentMethodBase)assessmentBase;

                    if (assessment.ShowAutoMessageOnStart)
                        WriteToConsole($"Checking {assessment.Title}\r\n");

                    var thisResult = await RunAssessmentAsync(assessment, assessmentBag);

                    if (thisResult)
                    {
                        if (assessment.ShowAutoMessageOnPass)
                            WriteToConsole(assessment.Title + " passed\r\n");
                    }
                    else // Assessment failed
                    {
                        groupResult = false;

                        if (assessment.ShowAutoMessageOnFail)
                            WriteToConsole(assessment.Title + " failed\r\n");

                        if (assessment.EndAssessmentGroupOnFail) break;
                    }
                } // End of assessment loop

                if (groupResult)
                {
                    if (assessmentGroup.ShowAutoMessageOnPass)
                        WriteToConsole(assessmentGroup.Title + " passed\r\n");
                }
                else // Assessment group failed
                {
                    result = false;

                    if (assessmentGroup.ShowAutoMessageOnFail)
                        WriteToConsole(assessmentGroup.Title + " failed\r\n");

                    if (assessmentGroup.EndAssessmentsOnFail) break;
                }
            } // End of assessment group loop

            if (result)
            {
                WriteToConsole("Congratulations!");
            }

            return result;
        }

        private async Task<bool> RunAssessmentAsync(AssessmentMethodBase assessment, dynamic assessmentBag)
        {
            assessment.AssessmentBag = assessmentBag;
            assessment.ConsoleWrite += OnConsoleWrite;
            if (assessment is AssessmentByInspectionBase inspectionAssessment)
            {
                inspectionAssessment.Compilation = compilation;
            }
            else if (assessment is AssessmentByRunningBase runningAssessment)
            {
                if (diags.Any(d => d.Severity == DiagnosticSeverity.Error))
                {
                    WriteToConsole("Unable to run your code because it has compiler errors\r\n");
                    return false;
                }

                runningAssessment.CodeRunner = runner;
                try
                {
                    runningAssessment.CompiledCode = await compiledCode.Value;
                }
                catch (Exception e) when (!(e is CompilationErrorException))
                {
                    WriteToConsole("Something went wrong with the compilation\r\n");
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
