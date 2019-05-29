using CodingTrainer.CodingTrainerModels;
using CodingTrainer.CodingTrainerModels.Security;
using CodingTrainer.CodingTrainerWeb.Dependencies;
using CodingTrainer.CSharpRunner.CodeHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CodingTrainer.CSharpRunner.Assessment
{
    public class AssessmentManager
    {
        public event ConsoleWriteEventHandler ConsoleWrite;

        private readonly ICodeRunner codeRunner;
        private readonly ICodingTrainerRepository rep;
        private readonly IUserServices userServices;

        public AssessmentManager(ICodeRunner runner, ICodingTrainerRepository repository, IUserServices userServices)
        {
            codeRunner = runner;
            rep = repository;
            this.userServices = userServices;
        }

        public async Task<bool> RunAssessmentsForExercise(ApplicationUser user, string code, int chapter, int exercise)
        {
            var output = new StringBuilder();
            Exercise exerciseDetails;
            string userCode;
            var result = false;

            if (!user.ExercisePermitted(chapter, exercise))
            {
                WriteToConsole("You do not have permission to do this exercise\r\n");
                return false;
            }
            if ((exerciseDetails = await rep.GetExerciseAsync(chapter, exercise)) == null)
            {
                WriteToConsole("Invalid exercise details\r\n");
                return false;
            }
            if (!code.StartsWith(exerciseDetails.HiddenCodeHeader))
            {
                WriteToConsole("Something went wrong with the pre-prepared code header");
                throw new InvalidOperationException("Start of user's code does not match the hidden code header");
            }
            userCode = code.Substring(exerciseDetails.HiddenCodeHeader.Length + 1);

            try
            {
                var assessmentGroups = await rep.GetAssessmentGroupsForExerciseAsync(chapter, exercise);
                var assessmentRunner = new AssessmentRunner(codeRunner, code);
                assessmentRunner.ConsoleWrite += OnConsoleWrite;
                result = await assessmentRunner.RunAssessmentsAsync(assessmentGroups);
                assessmentRunner.ConsoleWrite -= OnConsoleWrite;
            }
            finally
            {
                var savedAssessment = new Submission
                {
                    UserId = user.Id,
                    ChapterNo = chapter,
                    ExerciseNo = exercise,
                    SubmittedCode = userCode,
                    Output = output.ToString(),
                    Success = result,
                    SubmissionDateTime = DateTimeOffset.UtcNow
                };
                await rep.InsertSubmissionAsync(savedAssessment);
            }

            void WriteToConsole(string message)
            {
                OnConsoleWrite(this, new ConsoleWriteEventArgs(message));
            }

            void OnConsoleWrite(object sender, ConsoleWriteEventArgs e)
            {
                output.Append(e.Message);
                ConsoleWrite?.Invoke(sender, e);
            }

            if (result)
            {
                await userServices.AdvanceToExercise(await rep.GetNextExerciseAsync(chapter, exercise));
            }

            return result;
        }
    }
}