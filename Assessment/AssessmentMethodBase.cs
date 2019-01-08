using CodingTrainer.CSharpRunner.CodeHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTrainer.CSharpRunner.Assessment
{
    internal abstract class AssessmentMethodBase
    {
        public event ConsoleWriteEventHandler ConsoleWrite;

        protected CompiledCode CompiledCode { get; }
        protected string Title { get; }

        protected AssessmentMethodBase(string title, CompiledCode compiledCode)
        {
            Title = title;
            CompiledCode = compiledCode;
        }

        public virtual async Task<bool> AssessAsync()
        {
            DisplayStartMessage();
            var result = await DoAssessmentAsync();
            DisplayEndMessage(result);
            return result;
        }

        protected virtual void DisplayStartMessage()
        {
            WriteToConsole($"Starting test: {Title}...");
        }

        protected abstract Task<bool> DoAssessmentAsync();

        protected virtual void DisplayEndMessage(bool success)
        {
            WriteToConsole(success ? "Test passed!" : "Test failed!");
        }

        protected void WriteToConsole(string s)
        {
            ConsoleWrite?.Invoke(this, new ConsoleWriteEventArgs(s));
        }
    }
}
