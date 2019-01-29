using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimMetricsMetricUtilities;

namespace CodingTrainer.CSharpRunner.Assessment.Methods.ByRunning
{
    public class CheckLastLineOfOutputAssessment : AssessmentByRunningBase
    {
        [Required]
        public string ExpectedResult { get; set; }
        [Required]
        public double RequiredAccuracy { get; set; }

        protected override bool CheckResult(string consoleOut)
        {
            var lines = consoleOut.Split(new string[] { "\r", "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            var lastLine = lines[lines.Length - 1];

            if (RequiredAccuracy == 1)
            {
                // No differences allowed
                return lastLine == ExpectedResult;
            }

            var comparer = new Levenstein();
            var accuracy = comparer.GetSimilarity(lastLine, ExpectedResult);
            return (accuracy >= RequiredAccuracy);
        }
    }
}
