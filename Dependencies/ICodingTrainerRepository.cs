using CodingTrainer.CodingTrainerModels;
using CodingTrainer.CodingTrainerModels.Security;
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
        Task<Exercise> GetExerciseAsync(int chapterNo, int exerciseNo);
        Exercise GetExercise(int chapterNo, int exerciseNo);
        Task<Exercise> GetNextExerciseAsync(int chapterNo, int exerciseNo);

        // Saved work
        Task SaveWorkAsync(SavedWork savedWork);
        Task<SavedWork> GetSavedWorkAsync(int chapter, int exercise, string userId);

        // Assessments
        Task<IEnumerable<AssessmentGroup>> GetAssessmentGroupsForExerciseAsync(int chapterNo, int exerciseNo);
        Task InsertSubmissionAsync(Submission submission);
        Task<IEnumerable<Submission>> GetSubmissionsAsync(int chapterNo, int exerciseNo, string userId);
        Task<Submission> GetSubmissionAsync(int submissionId);
        Submission GetSubmission(int submissionId);

        // Exception logs
        Task InsertExceptionLogAsync(ExceptionRunningUsersCode log);

        // Unhandled exceptions
        void InsertUnhandledControllerException(UnhandledControllerException exception);

        // Users
        Task<IEnumerable<ApplicationUser>> GetUsersAsync();
    }
}
