using CodingTrainer.CSharpRunner.CodeHost;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimMetricsMetricUtilities;

namespace CodingTrainer.CSharpRunner.Assessment.Methods.ByRunning
{
    public class CheckAllOutputAssessment:AssessmentByRunningBase
    {
        [Required]
        public string ExpectedResult { get; set; }
        [Required]
        public double RequiredAccuracy { get; set; }

        protected override bool CheckResult(string consoleOut)
        {
            // Normalise new line characters
            var fixedConsoleOut = consoleOut.Replace("\r\n", "\n").Trim();
            var fixedExpectedResult = ExpectedResult.Replace("\r\n", "\n").Trim();

            if (RequiredAccuracy == 1)
            {
                // No differences allowed
                return fixedConsoleOut == fixedExpectedResult;
            }

            var comparer = new Levenstein();
            var accuracy = comparer.GetSimilarity(fixedConsoleOut, fixedExpectedResult);
            return (accuracy >= RequiredAccuracy);
        }
    }
}
