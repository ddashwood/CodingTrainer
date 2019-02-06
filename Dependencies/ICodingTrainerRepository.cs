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

        // Saved work
        Task SaveWorkAsync(SavedWork savedWork);
        Task<SavedWork> GetSavedWorkAsync(int chapter, int exercise, string userId);

        // Assessments
        Task<IEnumerable<AssessmentBase>> GetAssessmentsMethodsForExerciseAsync(int chapterNo, int exerciseNo);
        Task InsertSubmissionAsync(Submission submission);
        Task<IEnumerable<Submission>> GetSubmissionsAsync(int chapterNo, int exerciseNo, string userId);

        // Exception logs
        Task InsertExceptionLogAsync(ExceptionRunningUsersCode log);

        // Unhandled exceptions
        void InsertUnhandledControllerException(UnhandledControllerException exception);
    }
}
