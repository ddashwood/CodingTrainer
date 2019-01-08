using CodingTrainer.CSharpRunner.CodeHost;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTrainer.CSharpRunner.Assessment
{
    internal abstract class AssessmentByRunningBase : AssessmentMethodBase
    {
        // Not mapped onto Entity Framework
        private bool codeRunnerSet = false;
        private CodeRunner codeRunner;
        [NotMapped]
        public CodeRunner CodeRunner
        {
            get
            {
                return codeRunner;
            }
            set
            {
                codeRunnerSet = true;
                codeRunner = value;
            }
        }

        // Entity Framework properties
        public string ConsoleInText { get; set; }
        public string ExpectedResult { get; set; }

        protected abstract bool CheckResult(string consoleOut);

        protected sealed override async Task<bool> DoAssessmentAsync()
        {
            if (!codeRunnerSet) throw new InvalidOperationException("Attempt to run assessment without a code runner");

            StringBuilder console = new StringBuilder();
            void OnConsoleWrite(object sender, ConsoleWriteEventArgs e)
            {
                WriteToConsole(e.Message);
                console.Append(e.Message);
            }

            CodeRunner.ConsoleWrite += OnConsoleWrite;
            await CodeRunner.RunAsync(CompiledCode, new PreProgrammedTextReader(ConsoleInText));
            CodeRunner.ConsoleWrite -= OnConsoleWrite;

            return CheckResult(console.ToString());
        }
    }
}
