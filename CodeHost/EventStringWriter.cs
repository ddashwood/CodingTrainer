using System;
using System.IO;

namespace CodingTrainer.CSharpRunner.CodeHost
{
    internal class EventStringWriter : StringWriter
    {
        public event EventHandler Flushed;

        public override void Flush()
        {
            base.Flush();
            Flushed?.Invoke(this, EventArgs.Empty);
        }

        public override void Write(bool value)
        {
            base.Write(value);
            Flush();
        }

        public override void Write(char value)
        {
            base.Write(value);
            Flush();
        }

        public override void Write(char[] buffer)
        {
            base.Write(buffer);
            Flush();
        }

        public override void Write(char[] buffer, int index, int count)
        {
            base.Write(buffer, index, count);
            Flush();
        }

        public override void Write(decimal value)
        {
            base.Write(value);
            Flush();
        }

        public override void Write(double value)
        {
            base.Write(value);
            Flush();
        }

        public override void Write(float value)
        {
            base.Write(value);
            Flush();
        }

        public override void Write(int value)
        {
            base.Write(value);
            Flush();
        }

        public override void Write(long value)
        {
            base.Write(value);
            Flush();
        }

        public override void Write(object value)
        {
            base.Write(value);
            Flush();
        }

        public override void Write(string format, object arg0)
        {
            base.Write(format, arg0);
            Flush();
        }

        public override void Write(string format, object arg0, object arg1)
        {
            base.Write(format, arg0, arg1);
            Flush();
        }

        public override void Write(string format, object arg0, object arg1, object arg2)
        {
            base.Write(format, arg0, arg1, arg2);
            Flush();
        }

        public override void Write(string format, params object[] arg)
        {
            base.Write(format, arg);
            Flush();
        }

        public override void Write(string value)
        {
            base.Write(value);
            Flush();
        }

        public override void Write(uint value)
        {
            base.Write(value);
            Flush();
        }

        public override void Write(ulong value)
        {
            base.Write(value);
            Flush();
        }

        public override void WriteLine()
        {
            base.WriteLine();
            Flush();
        }

        public override void WriteLine(bool value)
        {
            base.WriteLine(value);
            Flush();
        }

        public override void WriteLine(char value)
        {
            base.WriteLine(value);
            Flush();
        }

        public override void WriteLine(char[] buffer)
        {
            base.WriteLine(buffer);
            Flush();
        }

        public override void WriteLine(char[] buffer, int index, int count)
        {
            base.WriteLine(buffer, index, count);
            Flush();
        }

        public override void WriteLine(decimal value)
        {
            base.WriteLine(value);
            Flush();
        }

        public override void WriteLine(double value)
        {
            base.WriteLine(value);
            Flush();
        }

        public override void WriteLine(float value)
        {
            base.WriteLine(value);
            Flush();
        }

        public override void WriteLine(int value)
        {
            base.WriteLine(value);
            Flush();
        }

        public override void WriteLine(long value)
        {
            base.WriteLine(value);
            Flush();
        }

        public override void WriteLine(object value)
        {
            base.WriteLine(value);
            Flush();
        }

        public override void WriteLine(string format, object arg0)
        {
            base.WriteLine(format, arg0);
            Flush();
        }

        public override void WriteLine(string format, object arg0, object arg1)
        {
            base.WriteLine(format, arg0, arg1);
            Flush();
        }

        public override void WriteLine(string format, object arg0, object arg1, object arg2)
        {
            base.WriteLine(format, arg0, arg1, arg2);
            Flush();
        }

        public override void WriteLine(string format, params object[] arg)
        {
            base.WriteLine(format, arg);
            Flush();
        }

        public override void WriteLine(string value)
        {
            base.WriteLine(value);
            Flush();
        }

        public override void WriteLine(uint value)
        {
            base.WriteLine(value);
            Flush();
        }

        public override void WriteLine(ulong value)
        {
            base.WriteLine(value);
            Flush();
        }
    }
}
