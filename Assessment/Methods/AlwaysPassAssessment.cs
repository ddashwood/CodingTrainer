using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTrainer.CSharpRunner.Assessment.Methods
{
    public class AlwaysPassAssessment : AssessmentByInspectionBase
    {
        protected override Task<bool> AssessCompilationAsync(Compilation compilation)
        {
            return Task.FromResult(true);
        }
    }
}
