using System;

namespace CodingTrainer.CSharpRunner.CodeHost
{
    public interface IExceptionLogger
    {
        void LogException(Exception e, string code);
    }
}
