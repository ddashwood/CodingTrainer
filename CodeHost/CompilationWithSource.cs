using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTrainer.CSharpRunner.CodeHost
{
    public struct CompilationWithSource
    {
        internal CompilationWithSource(Compilation compilation, string code)
        {
            Compilation = compilation;
            Code = code;
        }

        public Compilation Compilation { get; }
        public string Code { get; }
    }
}
