using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTrainer.CodingTrainerWeb.Hubs
{
    public interface ICodeRunnerHubClient
    {
        void ConsoleOut(string message);
        void Complete();
    }
}
