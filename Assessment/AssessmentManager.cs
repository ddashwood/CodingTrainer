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

        public AssessmentManager(ICodeRunner runner, ICodingTrainerRepository repository)
        {
            codeRunner = runner;
            rep = repository;
        }

        public async Task RunAssessmentsForExercise(ApplicationUser user, string code, int chapter, int exercise)
        {
            var output = new StringBuilder();

            if (!user.ExercisePermitted(chapter, exercise))
            {
                WriteToConsole("You do not have permission to do this exercise\r\n");
                return;
            }
            if (await rep.GetExerciseAsync(chapter, exercise) == null)
            {
                WriteToConsole("Invalid exercise details\r\n");
                return;
            }

            var assessments = await rep.GetAssessmentsMethodsForExerciseAsync(chapter, exercise);
            var assessmentRunner = new AssessmentRunner(codeRunner);
            assessmentRunner.ConsoleWrite += OnConsoleWrite;
            var result = await assessmentRunner.RunAssessmentsAsync(code, assessments.Select(a => (AssessmentMethodBase)a));
            assessmentRunner.ConsoleWrite -= OnConsoleWrite;

            var savedAssessment = new Submission
            {
                UserId = user.Id,
                ChapterNo = chapter,
                ExerciseNo = exercise,
                SubmittedCode = code,
                Output = output.ToString(),
                Success = result,
                SubmissionDateTime = DateTime.Now
            };
            await rep.InsertSubmissionAsync(savedAssessment);

            void WriteToConsole(string message)
            {
                OnConsoleWrite(this, new ConsoleWriteEventArgs(message));
            }

            void OnConsoleWrite(object sender, ConsoleWriteEventArgs e)
            {
                output.Append(e.Message);
                ConsoleWrite?.Invoke(sender, e);
            }
        }
    }
}