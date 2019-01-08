using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTrainer.CSharpRunner.CodeHost
{
    public interface ICodeRunner
    {
        event ConsoleWriteEventHandler ConsoleWrite;

        Task CompileAndRun(string code);
        Task<CompiledCode> Compile(string code);
        Task Run(CompiledCode compiledCode);
        Task Run(CompiledCode compiledCode, TextReader consoleInTextReader);

        void ConsoleIn(string text);
    }
}
