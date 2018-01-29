using System;
using System.Threading.Tasks;

namespace CodingTrainer.CSharpRunner.CodeHost
{
    public interface IExceptionLogger
    {
        Task LogException(Exception e, string code);
    }
}
