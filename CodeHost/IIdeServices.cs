using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CodingTrainer.CSharpRunner.CodeHost
{
    public interface IIdeServices
    {
        Task<IEnumerable<Diagnostic>> GetDiagnostics(string code, CancellationToken token = default(CancellationToken));
        Task<IEnumerable<string>> GetCompletions(string code, int position, CancellationToken token = default(CancellationToken));
    }
}
