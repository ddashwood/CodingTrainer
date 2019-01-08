using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTrainer.CSharpRunner.CodeHost
{
    public struct CompiledCode
    {
        internal CompiledCode(string source, byte[] bin, byte[] pdb)
        {
            this.source = source;
            this.bin = bin;
            this.pdb = pdb;
        }

        internal string source;
        internal byte[] bin;
        internal byte[] pdb;
    }
}
