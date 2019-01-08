using CodingTrainer.CSharpRunner.CodeHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTrainer.CSharpRunner.Assessment
{
    internal abstract class AssessmentByRunningBase:AssessmentMethodBase
    {
        private readonly CodeRunner codeRunner;
        private readonly string consoleInText;

        protected AssessmentByRunningBase(string title, CompiledCode compiledCode, CodeRunner codeRunner, string consoleInText)
            :base(title, compiledCode)
        {
            this.codeRunner = codeRunner;
            this.consoleInText = consoleInText;
        }

        protected abstract bool CheckResult(string consoleOut);

        protected sealed override async Task<bool> DoAssessmentAsync()
        {
            StringBuilder console = new StringBuilder();
            void OnConsoleWrite(object sender, ConsoleWriteEventArgs e)
            {
                WriteToConsole(e.Message);
                console.Append(e.Message);
            }

            codeRunner.ConsoleWrite += OnConsoleWrite;
            await codeRunner.RunAsync(CompiledCode, new PreProgrammedTextReader(consoleInText));
            codeRunner.ConsoleWrite -= OnConsoleWrite;

            return CheckResult(console.ToString());
        }
    }
}
