using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTrainer.CodingTrainerWeb.Hubs
{
    interface ICodeRunnerHubServer
    {
        Task Run(string code);
        Task Assess(string code, int chapter, int exercise);
        void ConsoleIn(string message);
    }
}
