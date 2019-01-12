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
            CompilationObject = compilation;
            Code = code;
        }

        public Compilation CompilationObject { get; }
        public string Code { get; }
    }
}
