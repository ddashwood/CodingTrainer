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
        void ConsoleIn(string message);

    }
}
