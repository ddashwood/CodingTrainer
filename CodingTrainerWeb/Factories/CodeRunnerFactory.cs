using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CodingTrainer.CSharpRunner.CodeHost;

namespace CodingTrainer.CodingTrainerWeb.Factories
{
    public class CodeRunnerFactory : ICodeRunnerFactory
    {
        public ICodeRunner GetCodeRunner()
        {
            return new CodeRunner();
        }
    }
}