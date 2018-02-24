using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTrainer.CodingTrainerWeb.Hubs
{
    public interface IIdeHubServer
    {
        Task RequestDiags(string code, int generation);
        Task RequestCompletions(string code, int cursorPosition, int tokenStart);
    }
}
