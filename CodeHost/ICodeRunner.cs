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

        Task CompileAndRunAsync(string code);
        Task<CompiledCode> CompileAsync(string code);
        Task<CompilationWithSource> GetCompilationAsync(string code);
        Task<CompiledCode> EmitFromCompilationAsync(CompilationWithSource compilation);

        Task RunAsync(CompiledCode compiledCode);
        Task RunAsync(CompiledCode compiledCode, TextReader consoleInTextReader);

        void ConsoleIn(string text);
    }
}
