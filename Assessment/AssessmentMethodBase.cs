using CodingTrainer.CodingTrainerModels;
using CodingTrainer.CSharpRunner.CodeHost;
using Microsoft.CodeAnalysis;
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
        static AssessmentMethodBase()
        {
            // Add types to Dynamic Linq which might be used by assessment queries.
            // This has to happen once, before assessments run - doing it in the
            // static constructor should do the trick

            // Private type, hence we can't simply use typeof
            Type type = typeof(System.Linq.Dynamic.DynamicQueryable).Assembly.GetType("System.Linq.Dynamic.ExpressionParser");
            FieldInfo field = type.GetField("predefinedTypes", BindingFlags.Static | BindingFlags.NonPublic);

            Type[] predefinedTypes = (Type[])field.GetValue(null);

            Array.Resize(ref predefinedTypes, predefinedTypes.Length + 1);
            predefinedTypes[predefinedTypes.Length - 1] = typeof(SyntaxToken);

            field.SetValue(null, predefinedTypes);

            field = type.GetField("keywords", BindingFlags.Static | BindingFlags.NonPublic);
            field.SetValue(null, null);
        }

        // Events
        public event ConsoleWriteEventHandler ConsoleWrite;

        public override async Task<bool> AssessAsync()
        {
            DisplayStartMessage();
            bool result = true;
            try
            {
                result = await DoAssessmentAsync();
                DisplayEndMessage(result);
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

        protected virtual void DisplayStartMessage()
        {
            WriteToConsole($"Starting test: {Title}...\r\n");
        }

        protected abstract Task<bool> DoAssessmentAsync();

        protected virtual void DisplayEndMessage(bool success)
        {
            WriteToConsole(success ? "Test passed!\r\n\r\n" : "Test failed!\r\n\r\n");
        }

        protected void WriteToConsole(string s)
        {
            ConsoleWrite?.Invoke(this, new ConsoleWriteEventArgs(s));
        }
    }
}
