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
        private IExceptionLogger exceptionLogger;

        public AssessmentRunner()
        { }
        public AssessmentRunner(IExceptionLogger exceptionLogger)
        {
            this.exceptionLogger = exceptionLogger;
        }

        public async Task<bool> RunAssessmentsAsync(string code, IEnumerable<AssessmentMethodBase> assessments)
        {
            CompiledCode compiledCode;
            var runner = new CodeRunner(exceptionLogger);
            try
            {
                compiledCode = await runner.CompileAsync(code);
            }
            catch (CompilationErrorException e)
            {
                // TO DO - Report the error to the user
                return false;
            }
            catch (Exception e)
            {
                // TO DO - Report the error to the user
                return false;
            }

            var result = true;
            foreach (var assessment in assessments)
            {
                assessment.CompiledCode = compiledCode;
                assessment.ConsoleWrite += OnConsoleWrite;
                if (assessment is AssessmentByRunningBase runningAssessment)
                {
                    runningAssessment.CodeRunner = runner;
                }
                var thisResult = await assessment.AssessAsync();
                if (!thisResult) result = false;

                // TO DO - Decide whether to carry on?
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
