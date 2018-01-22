using System;
using System.IO;
using System.Threading;

namespace CodingTrainer.CSharpRunner.CodeHost
{
    internal class BlockingMemoryStream:MemoryStream
    {
        private ManualResetEvent dataReady = new ManualResetEvent(false);
        private object streamLock = new object();

        public override bool CanRead => true;

        public BlockingMemoryStream()
        {
            Position = 0;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int retCount;
            lock (streamLock)
            {
                retCount = base.Read(buffer, offset, count);
            }
            if (retCount == 0)
            {
                dataReady.Reset();
                dataReady.WaitOne();
                lock (streamLock)
                {
                    retCount = base.Read(buffer, offset, count);
                }
            }
            return retCount;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            lock (streamLock)
            {
                long oldPosition = Position;
                Position = Length;

                base.Write(buffer, offset, count);

                Position = oldPosition;
            }
            if (count > 0) dataReady.Set();
        }
    }
}
