using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Completion;
using Microsoft.CodeAnalysis.Host;
using Microsoft.CodeAnalysis.Recommendations;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CodingTrainer.CSharpRunner.CodeHost
{
    public class IdeServices : IIdeServices
    {
        public async Task<IEnumerable<Diagnostic>> GetDiagnostics(string code, CancellationToken token)
        {
            var compilation = await Compiler.GetCompilation(code);
            token.ThrowIfCancellationRequested();
            var diagnostics = await GetDiagnostics(compilation, token);
            token.ThrowIfCancellationRequested();

            return diagnostics;
        }

        private static async Task<IEnumerable<Diagnostic>> GetDiagnostics(Compilation compilation, CancellationToken token = default(CancellationToken))
        {
            var any = false;
            var diagnostics = new List<Diagnostic>();

            // Will we ever have more than one syntax tree? Probably not if we're just compiling
            // a script, but use a foreach just in case
            foreach (var tree in compilation.SyntaxTrees)
            {
                token.ThrowIfCancellationRequested();
                var treeDiagnostics = await Task.Run(() => compilation.GetSemanticModel(tree).GetDiagnostics());
                if (treeDiagnostics.Length > 0)
                {
                    any = true;
                    diagnostics.AddRange(treeDiagnostics);
                }
            }

            token.ThrowIfCancellationRequested();
            if (any) return diagnostics;
            return null;
        }

        public async Task<IEnumerable<string>> GetCompletions(string code, int position, CancellationToken token)
        {
            Workspace workspace = new AdhocWorkspace();
            List<string> symbolNames = new List<string>();

            var compilation = await Compiler.GetCompilation(code);
            foreach (var tree in compilation.SyntaxTrees)
            {
                SemanticModel sm = compilation.GetSemanticModel(tree);
                var symbols = await Recommender.GetRecommendedSymbolsAtPositionAsync(sm, position, workspace, cancellationToken: token);
                symbolNames.AddRange(symbols.OrderBy(s => s.Name).Select(s => s.Name).Distinct());
            }

            return symbolNames;
        }
    }
}
