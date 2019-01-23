using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodingTrainer.CSharpRunner.CodeHost;

namespace CodingTrainer.CSharpRunner.Assessment.Methods.ByRunning
{
    public class CheckLastLineOfOutputAssessment : AssessmentByRunningBase
    {
        [Required]
        public string ExpectedResult { get; set; }

        protected override bool CheckResult(string consoleOut)
        {
            var lines = consoleOut.Split(new string[] { "\r", "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            return lines[lines.Length - 1] == ExpectedResult;
        }
    }
}
