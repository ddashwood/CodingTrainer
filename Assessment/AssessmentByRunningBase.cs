using CodingTrainer.CSharpRunner.CodeHost;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CodingTrainer.CSharpRunner.Assessment
{
    [Table("AssessmentsByRunning")]
    public abstract class AssessmentByRunningBase : AssessmentMethodBase
    {
        // Not mapped onto Entity Framework
        [NotMapped]
        [XmlIgnore]
        public ICodeRunner CodeRunner { private get; set; }
        [NotMapped]
        [XmlIgnore]
        internal CompiledCode? CompiledCode { private get; set; }

        // Entity Framework properties
        // [Required] - Not required if UseResultFromPreviousAssessment = true
        public string ConsoleInText { get; set; }

        [Required]
        public bool ShowScriptRunning { get; set; }

        [Required]
        public bool UseResultFromPreviousAssessment { get; set; }

        protected abstract bool CheckResult(string consoleOut);

        protected sealed override async Task<bool> DoAssessmentAsync()
        {
            StringBuilder console;

            if (UseResultFromPreviousAssessment)
            {
                try
                {
                    console = AssessmentBag.Console;
                }
                catch (RuntimeBinderException)
                {
                    throw new InvalidOperationException("Attempt to use result from previous assessment on the first assessment-by-running");
                }
            }
            else
            {
                if (CodeRunner == null) throw new InvalidOperationException("Attempt to run assessment without a code runner");
                if (CompiledCode == null || CompiledCode.HasValue == false) throw new InvalidOperationException("Attempt to run assessment without any compiled code");

                console = new StringBuilder();
                void OnConsoleWrite(object sender, ConsoleWriteEventArgs e)
                {
                    if (ShowScriptRunning) WriteToConsoleHighlight(e.Message);
                    console.Append(e.Message);
                }

                CodeRunner.ConsoleWrite += OnConsoleWrite;
                try
                {
                    OnConsoleWrite(this, new ConsoleWriteEventArgs("\r\n")); // Separate from start/end messages
                    await CodeRunner.RunAsync(CompiledCode.Value, new PreProgrammedTextReader(ConsoleInText));
                    OnConsoleWrite(this, new ConsoleWriteEventArgs("\r\n"));
                }
                finally
                {
                    CodeRunner.ConsoleWrite -= OnConsoleWrite;
                }

                AssessmentBag.Console = console;
            }

            return CheckResult(console.ToString());
        }

        protected sealed override bool HandleExceptionInTest(Exception e)
        {
            if (e is AggregateException userCodeException)
            {
                return HandleExceptionInUsersCode(userCodeException);
            }
            else
            {
                return base.HandleExceptionInTest(e);
            }
        }

        protected virtual bool HandleExceptionInUsersCode(AggregateException e)
        {
            WriteToConsole("There was an exception when running your code\r\n");
            WriteToConsole("The exception message is:\r\n");
            WriteToConsole($"  {e.InnerException.Message}\r\n\r\n");

            throw e;
        }
    }
}
