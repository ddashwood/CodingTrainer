using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodingTrainer.CodingTrainerWeb.Hubs.Helpers;

namespace CodingTrainer.CodingTrainerWeb.Hubs
{
    public interface IIdeHubClient
    {
        void DiagsCallback(IEnumerable<CompilerError> diags, int generation);
        void CompletionsCallback(IEnumerable<string> completions, int generation);
    }
}
