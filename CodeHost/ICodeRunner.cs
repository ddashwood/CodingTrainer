using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTrainer.CSharpRunner.CodeHost
{
    public interface ICodeRunner
    {
        event ConsoleWriteEventHandler ConsoleWrite;

        Task RunCode(string code);
        void ConsoleIn(string text);
        Task<IEnumerable<Diagnostic>> GetDiagnostics(string code);
    }
}
