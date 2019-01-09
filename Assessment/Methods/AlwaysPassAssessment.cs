using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTrainer.CSharpRunner.Assessment.Methods
{
    class AlwaysPassAssessment : AssessmentByInspectionBase
    {
        protected override Task<bool> DoAssessmentAsync()
        {
            return Task.FromResult(true);
        }
    }
}
