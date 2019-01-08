using CodingTrainer.CSharpRunner.CodeHost;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CodingTrainer.CSharpRunner.Assessment
{
    public class PreProgrammedTextReader : StringReader, ITextWriterInjectable
    {
        public PreProgrammedTextReader(string s)
            : base(s)
        { }

        public TextWriter TextWriter { private get; set; }

        public override int Read()
        {
            int result = base.Read();
            if (result == -1) throw new EndOfStreamException("You have read too many lines from the console");

            TextWriter?.Write(char.ConvertFromUtf32(result));
            return result;
        }

        public override int Read([In, Out]  char[] buffer, int index, int count)
        {
            int result = base.Read(buffer, index, count);
            if (result == 0 && count != 0) throw new EndOfStreamException("You have read too many lines from the console");

            var output = new char[count];
            TextWriter?.Write(buffer.Skip(index).Take(result));
            return result;
        }

        public override string ReadToEnd()
        {
            if (Peek() == -1) throw new EndOfStreamException("You have read too many lines from the console");
            string result = base.ReadToEnd();
            TextWriter?.Write(result);
            return result;
        }

        public override string ReadLine()
        {
            if (Peek() == -1) throw new EndOfStreamException("You have read too many lines from the console");
            string result = base.ReadLine();
            TextWriter?.WriteLine(result);
            return result;
        }
    }
}
