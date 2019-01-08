using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTrainer.CSharpRunner.CodeHost
{
    public interface ITextWriterInjectable
    {
        TextWriter TextWriter { set; }
    }
}
