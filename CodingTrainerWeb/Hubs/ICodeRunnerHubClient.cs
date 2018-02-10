using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodingTrainer.CodingTrainerWeb.Hubs.Helpers;

namespace CodingTrainer.CodingTrainerWeb.Hubs
{
    public interface ICodeRunnerHubClient
    {
        void ConsoleOut(string message);
        void Complete();
        void CompilerError(CompilerError[] details);
    }
}
