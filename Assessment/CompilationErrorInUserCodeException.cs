using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTrainer.CSharpRunner.Assessment
{
    class CompilationErrorInUserCodeException : ApplicationException
    {
        public CompilationErrorInUserCodeException()
        { }

        public CompilationErrorInUserCodeException(string message)
            :base(message)
        { }
    }
}
