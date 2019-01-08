using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodingTrainer.CSharpRunner.CodeHost;

namespace CodingTrainer.CSharpRunner.Assessment.Methods
{
    class CheckLastLineOfOutputAssessment : AssessmentByRunningBase
    {
        private readonly string expected;

        public CheckLastLineOfOutputAssessment(string title, CompiledCode compiledCode, CodeRunner codeRunner, string consoleInText, string expected)
            : base(title, compiledCode, codeRunner, consoleInText)
        {
            this.expected = expected;
        }

        protected override bool CheckResult(string consoleOut)
        {
            var lines = consoleOut.Split(new string[] { "\r", "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            return lines[lines.Length - 1] == expected;
        }
    }
}
