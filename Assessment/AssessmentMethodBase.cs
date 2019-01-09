using CodingTrainer.CodingTrainerModels;
using CodingTrainer.CSharpRunner.CodeHost;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTrainer.CSharpRunner.Assessment
{
    [Table("Assessment")]
    public abstract class AssessmentMethodBase:IAssessment
    {
        // Events
        public event ConsoleWriteEventHandler ConsoleWrite;

        // Not mapped onto Entity Framework
        private bool compiledCodeSet = false;
        private CompiledCode compiledCode;
        [NotMapped]
        internal CompiledCode CompiledCode
        {
            get
            {
                return compiledCode;
            }
            set
            {
                compiledCode = value;
                compiledCodeSet = true;
            }
        }

        // Entity Framework properties
        [Key, Required]
        public int AssessmentId { get; set; }

        [Required]
        public int ChapterNo { get; set; }

        [Required]
        public int ExerciseNo { get; set; }

        [Required]
        public string Title { get; set; }

        public virtual Exercise Exercise { get; set; }

        public virtual async Task<bool> AssessAsync()
        {
            if (!compiledCodeSet) throw new InvalidOperationException("Attempt to run assessment without any compiled code");

            DisplayStartMessage();
            var result = await DoAssessmentAsync();
            DisplayEndMessage(result);
            return result;
        }

        protected virtual void DisplayStartMessage()
        {
            WriteToConsole($"Starting test: {Title}...\r\n");
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
