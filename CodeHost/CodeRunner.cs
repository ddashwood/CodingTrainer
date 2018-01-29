using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Scripting;

// References:
//  Permissions, partially-trusted code and fully-trusted assemblies:
//    https://docs.microsoft.com/en-us/dotnet/framework/misc/how-to-run-partially-trusted-code-in-a-sandbox
//  Compiling and running code:
//    https://blog.jayway.com/2015/05/09/using-roslyn-to-build-a-simple-c-interactive-script-engine/
//
// Be sure to install Microsoft.CodeAnalysis.CSharp.Scripting package

namespace CodingTrainer.CSharpRunner.CodeHost
{
    public partial class CodeRunner:ICodeRunner
    {
        public event ConsoleWriteEventHandler ConsoleWrite;
        private TextWriter consoleInWriter;
        private IExceptionLogger exceptionLogger;

        public CodeRunner()
        { }
        public CodeRunner(IExceptionLogger exceptionLogger)
        {
            this.exceptionLogger = exceptionLogger;
        }

        public async Task RunCode(string code)
        {
            try
            {
                var compiled = await Compiler.Compile(code);
                new SandboxManager().RunInSandbox(this, compiled.result, compiled.pdb);
            }
            catch (Exception e) when (!(e is AggregateException || e is CompilationErrorException))
            {
                exceptionLogger?.LogException(e, code);
                throw;
            }
        }

        // Put some text into the console input stream
        public void ConsoleIn(string text)
        {
            if (consoleInWriter == null)
                throw new InvalidOperationException("Can't send input text to the console until the code is running");

            consoleInWriter.WriteLine(text);
            consoleInWriter.Flush();
        }
    }
}
