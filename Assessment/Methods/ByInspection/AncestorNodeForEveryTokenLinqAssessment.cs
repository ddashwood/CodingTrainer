using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using DynamicExpression = System.Linq.Dynamic.DynamicExpression;

namespace CodingTrainer.CSharpRunner.Assessment.Methods.ByInspection
{
    [Obsolete("Use SyntaxTreeScriptAssessment instead", true)]
    public class AncestorNodeForEveryTokenLinqAssessment : AssessmentByInspectionBase
    {
        public string TokenSelector { get; set; }
        public string ParentCondition { get; set; }

        protected override Task<bool> AssessCompilationAsync(Compilation compilation)
        {
            var myTokens = ExtractExpressionTreeTokens(compilation);

            var nodeParam = Expression.Parameter(typeof(SyntaxNode), "parentNode");
            var syntaxKindParam = Expression.Parameter(typeof(EnumHelper<SyntaxKind, ushort>), "syntaxKind");
            var syntaxKind = new EnumHelper<SyntaxKind, ushort>();

            var result = true;
            foreach (var token in myTokens)
            {
                var resultForThisToken = SearchTokenTreeForItem(token, nodeParam, syntaxKindParam, syntaxKind);
                if (resultForThisToken == false)
                {
                    result = false;
                    break;
                }
            }


            return Task.FromResult(result);
        }

        private IEnumerable<SyntaxToken> ExtractExpressionTreeTokens(Compilation compilation)
        {
            var tokensParam = Expression.Parameter(typeof(IEnumerable<SyntaxToken>), "tokens");
            var expression =
                DynamicExpression.ParseLambda(new[] {tokensParam}, typeof(IEnumerable<SyntaxToken>), TokenSelector);

            var tree = compilation.SyntaxTrees.Single();
            var tokens = tree.GetRoot().DescendantTokens(c => true);
            var myTokens = (IEnumerable<SyntaxToken>) expression.Compile().DynamicInvoke(tokens);
            return myTokens;
        }

        private bool SearchTokenTreeForItem(SyntaxToken token, ParameterExpression nodeParam,
            ParameterExpression syntaxKindParam, EnumHelper<SyntaxKind, ushort> syntaxKind)
        {
            var resultForThisToken = false;
            var parent = token.Parent;
            while (parent != null)
            {
                var parentExpression =
                    DynamicExpression.ParseLambda(new[] {nodeParam, syntaxKindParam}, typeof(bool), ParentCondition);
                var thisResult = (bool) parentExpression.Compile().DynamicInvoke(parent, syntaxKind);

                if (thisResult)
                {
                    resultForThisToken = true;
                    break;
                }

                parent = parent.Parent;
            }

            return resultForThisToken;
        }
    }
}
