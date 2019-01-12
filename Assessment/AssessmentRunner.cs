using CodingTrainer.CSharpRunner.Assessment.Methods;
using CodingTrainer.CSharpRunner.CodeHost;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTrainer.CSharpRunner.Assessment
{
    public class AssessmentRunner
    {
        public event ConsoleWriteEventHandler ConsoleWrite;
        private ICodeRunner runner;

        public AssessmentRunner(ICodeRunner runner)
        {
            this.runner = runner;
        }

        public async Task<bool> RunAssessmentsAsync(string code, IEnumerable<AssessmentMethodBase> assessments)
        {
            // Prepare to compile the code

            LazyAsync<CompiledCode> compiledCode;
            CompilationWithSource compilation;
            try
            {
                compilation = await runner.GetCompilationAsync(code);
                var tree = compilation.CompilationObject.SyntaxTrees.Single();
                var diags = compilation.CompilationObject.GetSemanticModel(tree).GetDiagnostics();
                var errors = diags.Any(d => d.Severity == DiagnosticSeverity.Error);
                if (errors) throw new CompilationErrorException("Error compiling your code", diags.ToImmutableArray());

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
            var aborted = false;
            var failCount = 0;
            foreach (var assessment in assessments)
            {
                assessment.ConsoleWrite += OnConsoleWrite;
                if (assessment is AssessmentByInspectionBase inspectionAssessment)
                {
                    inspectionAssessment.Compilation = compilation;
                }
                else if (assessment is AssessmentByRunningBase runningAssessment)
                {
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
                try
                {
                    var thisResult = await assessment.AssessAsync();

                    if (!thisResult)
                    {
                        result = false;
                        failCount++;
                        if (assessment.AbortOnFail)
                        {
                            WriteToConsole("No further tests will be run, please fix this test then re-submit\r\n");
                            aborted = true;
                            break;
                        }
                    }
                }
                catch
                {
                    WriteToConsole("No further tests will be run, due to errors in this test\r\n");
                    result = false;
                    aborted = true;
                    failCount++;
                    break;
                }
            }

            if (result)
            {
                WriteToConsole("Congratulations!");
            }
            else if (!aborted)
            {
                WriteToConsole($"There are still {failCount} tests which have not passed");
            }

            return result;
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
