using CodingTrainer.CSharpRunner.CodeHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTrainer.CSharpRunner.CodeHost.Factories
{
    public interface ICodeRunnerFactory
    {
        ICodeRunner GetCodeRunner();
    }
}
