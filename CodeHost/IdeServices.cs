using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Completion;
using Microsoft.CodeAnalysis.Host;
using Microsoft.CodeAnalysis.Recommendations;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.CSharp;
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
        ICodeRunner runner;
        public IdeServices(ICodeRunner runner)
        {
            this.runner = runner;
        }
        public async Task<IEnumerable<Diagnostic>> GetDiagnosticsAsyc(string code, CancellationToken token)
        {
            var compilation = await runner.GetCompilationAsync(code);
            token.ThrowIfCancellationRequested();
            var diagnostics = await GetDiagnostics(compilation.CompilationObject, token);
            token.ThrowIfCancellationRequested();

            return diagnostics;
        }

        private static async Task<IEnumerable<Diagnostic>> GetDiagnostics(Compilation compilation, CancellationToken token = default(CancellationToken))
        {
            var any = false;
            var diagnostics = new List<Diagnostic>();

            var tree = compilation.SyntaxTrees.Single();
            token.ThrowIfCancellationRequested();
            var treeDiagnostics = await Task.Run(() => compilation.GetSemanticModel(tree).GetDiagnostics());
            if (treeDiagnostics.Length > 0)
            {
                any = true;
                diagnostics.AddRange(treeDiagnostics);
            }

            token.ThrowIfCancellationRequested();
            if (any) return diagnostics;
            return null;
        }

        public async Task<IEnumerable<string>> GetCompletionStringsAsync(string code, int position, CancellationToken token)
        {
            Workspace workspace = new AdhocWorkspace();

            var compilation = await runner.GetCompilationAsync(code);
            var tree = compilation.CompilationObject.SyntaxTrees.Single();

            SemanticModel sm = compilation.CompilationObject.GetSemanticModel(tree);
            var symbols = await Recommender.GetRecommendedSymbolsAtPositionAsync(sm, position, workspace, cancellationToken: token);

            return symbols.OrderBy(s => s.Name).Select(s => s.Name).Distinct();
        }

        public async Task<IEnumerable<ISymbol>> GetOverloadsAndParametersAsync(string code, int position, CancellationToken token = default(CancellationToken))
        {
            var compilation = await runner.GetCompilationAsync(code);
            var tree = compilation.CompilationObject.SyntaxTrees.Single();

            var semanticModel = compilation.CompilationObject.GetSemanticModel(tree);
            token.ThrowIfCancellationRequested();

            var theToken = (await tree.GetRootAsync(token)).FindToken(position);
            var theNode = theToken.Parent;
            bool foundArgumentList = false;
            while (!theNode.IsKind(SyntaxKind.InvocationExpression))
            {
                if (theNode.IsKind(SyntaxKind.ArgumentList)) foundArgumentList = true;
                theNode = theNode.Parent;
                if (theNode == null) return null; // There isn't an InvocationExpression in this branch of the tree
            }
            if (!foundArgumentList) return null;  // There isn't an ArgumentList in this branch of the tree

            var symbolInfo = semanticModel.GetSymbolInfo(theNode);
            var symbol = symbolInfo.Symbol;
            var containingType = symbol.ContainingType;

            return containingType.GetMembers(symbol.Name);
        }
    }
}
