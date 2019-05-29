﻿using CodingTrainer.CodingTrainerWeb.Dependencies;
using CodingTrainer.CSharpRunner.Assessment;
using CodingTrainer.CSharpRunner.CodeHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace CodingTrainer.CodingTrainerWeb.Hubs
{
    public partial class CodeRunnerHub
    {
        private interface IConnectionTask
        {
            Task RunAsync(ConnectionOutput output, string message);
        }
        private interface IConnectionTaskWithInput : IConnectionTask
        {
            void Input(string message);
        }

        private class ConnectionCodeRunner : IConnectionTaskWithInput
        {
            private ICodeRunner runner;
            public ConnectionCodeRunner(ICodeRunner runner)
            {
                this.runner = runner;
            }

            public async Task RunAsync(ConnectionOutput output, string message)
            {
                runner.ConsoleWrite += output.OnConsoleWrite;
                await runner.CompileAndRunAsync(message);
                runner.ConsoleWrite -= output.OnConsoleWrite;
            }
            public void Input(string message)
            {
                runner.ConsoleIn(message);
            }
        }

        private class ConnectionAssessmentRunner : IConnectionTask
        {
            private readonly ICodeRunner runner;
            private readonly ICodingTrainerRepository rep;
            private readonly IUserServices userServices;
            private readonly int chapter;
            private readonly int exercise;

            public ConnectionAssessmentRunner(ICodeRunner runner, IUserServices userServices, ICodingTrainerRepository rep, int chapter, int exercise)
            {
                this.runner = runner;
                this.userServices = userServices;
                this.rep = rep;
                this.chapter = chapter;
                this.exercise = exercise;
            }
            public async Task RunAsync(ConnectionOutput output, string message)
            {
                try
                {
                    var assessmentManager = new AssessmentManager(runner, rep, userServices);
                    assessmentManager.ConsoleWrite += output.OnConsoleWrite;
                    var result = await assessmentManager.RunAssessmentsForExercise(userServices.GetCurrentUser(), message, chapter, exercise);
                    assessmentManager.ConsoleWrite -= output.OnConsoleWrite;

                    output.AssessmentComplete(result);
                }
                catch
                {
                    output.AssessmentComplete(false);
                    throw;
                }
            }
        }

    }
}