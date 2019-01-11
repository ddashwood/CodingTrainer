using CodingTrainer.CSharpRunner.Assessment.Methods;
using CodingTrainer.CSharpRunner.CodeHost;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
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
            // Compile the code first

            CompiledCode compiledCode;
            try
            {
                compiledCode = await runner.CompileAsync(code);
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
            var failCount = 0;
            foreach (var assessment in assessments)
            {
                assessment.CompiledCode = compiledCode;
                assessment.ConsoleWrite += OnConsoleWrite;
                if (assessment is AssessmentByRunningBase runningAssessment)
                {
                    runningAssessment.CodeRunner = runner;
                }
                try
                {
                    var thisResult = await assessment.AssessAsync();

                    if (!thisResult)
                    {
                        result = false;
                        failCount++;
                    }
                }
                catch
                {
                    WriteToConsole("No further tests will be run, due to errors in this test\r\n");
                    result = false;
                    failCount++;
                    break;
                }
            }

            if (result)
            {
                WriteToConsole("Congratulations!");
            }
            else
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
    }
}
