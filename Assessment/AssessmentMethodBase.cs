using CodingTrainer.CodingTrainerModels;
using CodingTrainer.CSharpRunner.CodeHost;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CodingTrainer.CSharpRunner.Assessment
{
    public abstract class AssessmentMethodBase:AssessmentBase
    {
        // Dynamic Linq can't use enum names, so this enables Dynamic Linq queries
        // to call the Value() method and pass the name as a parameter
        protected class EnumHelper<TEnum, TUnderlying> where TEnum:Enum
        {
            public EnumHelper()
            {
                if (!typeof(TUnderlying).IsAssignableFrom(Enum.GetUnderlyingType(typeof(TEnum))))
                    throw new InvalidOperationException("Enum type and underlying type do not match");
            }
            public TUnderlying Value(string name)
            {
                TEnum t = (TEnum)Enum.Parse(typeof(TEnum), name);
                return (TUnderlying)(object)t;
            }
        }

        // Not mapped onto Entity Framework
        [NotMapped]
        [IgnoreDataMember]
        public dynamic AssessmentBag { get; set; }

        // Events
        public event ConsoleWriteEventHandler ConsoleWrite;

        public override async Task<bool> AssessAsync()
        {
            bool result = true;
            try
            {
                result = await DoAssessmentAsync();
            }
            catch (Exception e)
            {
                HandleExceptionInTest(e);
            }
            return result;
        }

        /// <summary>
        /// Handles exceptions in a test
        /// </summary>
        /// <param name="e">The exception</param>
        /// <returns>True if the test passes, false if the test fails. Throw e if the test run should be aborted</returns>
        protected virtual bool HandleExceptionInTest(Exception e)
        {
            WriteToConsole("Something went wrong with this test\r\n");
            WriteToConsole("The error message is:\r\n  ");
            WriteToConsole(e.Message + "\r\n\r\n");

            throw e;
        }

        protected abstract Task<bool> DoAssessmentAsync();

        protected void WriteToConsole(string s)
        {
            ConsoleWrite?.Invoke(this, new ConsoleWriteEventArgs(s));
        }
        protected void WriteToConsoleHighlight(string s)
        {
            ConsoleWrite?.Invoke(this, new ConsoleWriteEventArgs(s, true));
        }
    }
}
