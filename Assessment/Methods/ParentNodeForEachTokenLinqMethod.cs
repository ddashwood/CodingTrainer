using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using DynamicExpression = System.Linq.Dynamic.DynamicExpression;

namespace CodingTrainer.CSharpRunner.Assessment.Methods
{
    public class ParentNodeForEveryTokenLinqMethod : AssessmentByInspectionBase
    {
        public string TokenSelector { get; set; }
        public string ParentCondition { get; set; }

        protected override Task<bool> DoAssessmentAsync()
        {
            var tree = Compilation.CompilationObject.SyntaxTrees.Single();
            var tokens = tree.GetRoot().DescendantTokens(c => true);

            var tokensParam = Expression.Parameter(typeof(IEnumerable<SyntaxToken>), "tokens");
            var expression = DynamicExpression.ParseLambda(new[] { tokensParam }, typeof(IEnumerable<SyntaxToken>), TokenSelector);

            var myTokens = (IEnumerable<SyntaxToken>)expression.Compile().DynamicInvoke(tokens);

            var nodeParam = Expression.Parameter(typeof(SyntaxNode), "parentNode");
            var syntaxKindParam = Expression.Parameter(typeof(EnumHelper<SyntaxKind, ushort>), "syntaxKind");
            var syntaxKind = new EnumHelper<SyntaxKind, ushort>();

            var result = true;
            foreach (var token in myTokens)
            {
                var parent = token.Parent;

                var parentExpression = DynamicExpression.ParseLambda(new[] { nodeParam, syntaxKindParam }, typeof(bool), ParentCondition);
                var thisResult = (bool)parentExpression.Compile().DynamicInvoke(parent, syntaxKind);
                if (!thisResult)
                {
                    result = false;
                    break;
                }
            }

            return Task.FromResult(result);
        }
    }
}
