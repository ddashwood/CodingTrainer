using CodingTrainer.CodingTrainerModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTrainer.CodingTrainerWeb.Dependencies
{
    public interface ICodingTrainerRepository
    {
        // Chapters
        Task<IEnumerable<Chapter>> GetAllChaptersAsync();

        // Exercises
        Task<Exercise> GetExerciseAsync(int chapterNo, int exercisesNo);
        Exercise GetExercise(int chapterNo, int exercisesNo);

        // Assessments
        Task<IEnumerable<AssessmentBase>> GetAssessmentsMethodsForExerciseAsync(int chapterNo, int exerciseNo);

        // Exception logs
        Task InsertExceptionLogAsync(ExceptionLog log);

        // Unhandled exceptions
        void InsertUnhandledControllerException(UnhandledControllerException exception);
    }
}
