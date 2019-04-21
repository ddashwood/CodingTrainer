using System.Collections.Generic;
using System.Threading.Tasks;
using CodingTrainer.CodingTrainerModels;

namespace CodingTrainer.CSharpRunner.Assessment
{
    public interface IAssessmentRunner
    {
        Task<bool> RunAssessmentsAsync(IEnumerable<AssessmentGroup> assessmentGroups);
    }
}