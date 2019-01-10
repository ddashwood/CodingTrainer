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
            // TO DO - Check user has permission to run exercise
            //         Check exercise actually exists, and is not the Playground!

            var assessments = await rep.GetAssessmentsMethodsForExerciseAsync(chapter, exercise);
            var assessmentRunner = new AssessmentRunner(codeRunner);
            assessmentRunner.ConsoleWrite += OnConsoleWrite;
            await assessmentRunner.RunAssessmentsAsync(code, assessments.Select(a => (AssessmentMethodBase)a));
            assessmentRunner.ConsoleWrite -= OnConsoleWrite;
        }

        private void OnConsoleWrite(object sender, ConsoleWriteEventArgs e)
        {
            ConsoleWrite?.Invoke(sender, e);
        }
    }
}