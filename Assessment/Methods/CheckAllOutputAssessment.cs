using CodingTrainer.CSharpRunner.CodeHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTrainer.CSharpRunner.Assessment.Methods
{
    class CheckAllOutputAssessment:AssessmentByRunningBase
    {
        protected override bool CheckResult(string consoleOut)
        {
            var lines = consoleOut.Split(new string[] { "\r", "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            return lines[lines.Length - 1] == ExpectedResult;
        }
    }
}
