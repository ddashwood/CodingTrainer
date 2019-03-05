using CodingTrainer.CSharpRunner.CodeHost;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTrainer.CSharpRunner.TestingCommon
{
    public abstract class CodeHostTestBase
    {
        #region Helpers
        protected string WrapInMain(string code)
        {
            StringBuilder codeBuilder = new StringBuilder();
            codeBuilder.Append("public static void Main() { ");
            codeBuilder.Append(code);
            codeBuilder.Append(" }");

            return codeBuilder.ToString();
        }

        protected static StringBuilder GetUsings(string[] usings)
        {
            StringBuilder codeBuilder = new StringBuilder();

            foreach (string ns in usings)
            {
                codeBuilder.Append($"using {ns}; ");
            }

            return codeBuilder;
        }
        #endregion

        #region Setup
        protected CodeRunner runner;
        protected StringBuilder console;

        [OneTimeSetUp]
        public virtual void OneTimeSetup()
        {
            OneTimeSetup(true);
        }
        protected virtual void OneTimeSetup(bool registerConsoleWrite)
        {
            runner = new CodeRunner();
            if (registerConsoleWrite)
                runner.ConsoleWrite += OnConsoleWrite;
        }

        [SetUp]
        public virtual void Setup()
        {
            console = new StringBuilder();
        }

        protected void OnConsoleWrite(object sender, ConsoleWriteEventArgs e)
        {
            console.Append(e.Message);
        }
        #endregion
    }
}
