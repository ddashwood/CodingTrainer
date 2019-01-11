using CodingTrainer.CodingTrainerModels.Security;
using CodingTrainer.CodingTrainerWeb.Dependencies;
using CodingTrainer.CSharpRunner.CodeHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace CodingTrainer.CSharpRunner.Assessment
{
    public class AssessmentManager
    {
        public event ConsoleWriteEventHandler ConsoleWrite;

        private ICodeRunner codeRunner;
        private ICodingTrainerRepository rep;

        public AssessmentManager(ICodeRunner runner, ICodingTrainerRepository repository)
        {
            codeRunner = runner;
            rep = repository;
        }

        public async Task RunAssessmentsForExercise(ApplicationUser user, string code, int chapter, int exercise)
        {
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
            await assessmentRunner.RunAssessmentsAsync(code, assessments.Select(a => (AssessmentMethodBase)a));
            assessmentRunner.ConsoleWrite -= OnConsoleWrite;
        }

        private void WriteToConsole(string message)
        {
            ConsoleWrite?.Invoke(this, new ConsoleWriteEventArgs(message));
        }

        private void OnConsoleWrite(object sender, ConsoleWriteEventArgs e)
        {
            ConsoleWrite?.Invoke(sender, e);
        }
    }
}