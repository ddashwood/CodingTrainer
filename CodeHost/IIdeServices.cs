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
        Task<IEnumerable<Diagnostic>> GetDiagnosticsAsyc(string code, CancellationToken token = default(CancellationToken));
        Task<IEnumerable<string>> GetCompletionStringsAsync(string code, int position, CancellationToken token = default(CancellationToken));
        Task<IEnumerable<ISymbol>> GetOverloadsAndParametersAsync(string code, int position, CancellationToken token = default(CancellationToken));
    }
}
