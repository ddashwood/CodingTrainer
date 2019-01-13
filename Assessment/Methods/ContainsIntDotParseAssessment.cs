using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.FindSymbols;
using DynamicExpression = System.Linq.Dynamic.DynamicExpression;

namespace CodingTrainer.CSharpRunner.Assessment.Methods
{
    public class ContainsIntDotParseAssessment : AssessmentByInspectionBase
    {
        protected override Task<bool> DoAssessmentAsync()
        {
            // EXAMPLE ONLY - Not for use in its current state!

            var tree = Compilation.CompilationObject.SyntaxTrees.Single();
            var model = Compilation.CompilationObject.GetSemanticModel(tree);
            var tokens = tree.GetRoot().DescendantTokens(c => true);

            bool result1, result2, result3;

            {
                // SyntaxTokensLinq
                var condition = "(tokens.Where(Text == \"Parse\" && GetNextToken(false, false, false, false).Text == \"(\").Count()) == 2";
                var tokensParam = Expression.Parameter(typeof(IEnumerable<SyntaxToken>), "tokens");
                var expression = DynamicExpression.ParseLambda(new[] { tokensParam }, typeof(bool), condition);

                result1 = (bool)expression.Compile().DynamicInvoke(tokens);
            }


            {
                // ParentNodeForEachToken
                var tokenSelector = "tokens.Where(Text == \"Parse\" && GetNextToken(false, false, false, false).Text == \"(\")";
                var tokensParam = Expression.Parameter(typeof(IEnumerable<SyntaxToken>), "tokens");
                var expression = DynamicExpression.ParseLambda(new[] { tokensParam }, typeof(IEnumerable<SyntaxToken>), tokenSelector);

                var myTokens = (IEnumerable<SyntaxToken>)expression.Compile().DynamicInvoke(tokens);

                var nodeParam = Expression.Parameter(typeof(SyntaxNode), "parentNode");
                var syntaxKindParam = Expression.Parameter(typeof(EnumHelper<SyntaxKind, ushort>), "syntaxKind");
                var syntaxKind = new EnumHelper<SyntaxKind, ushort>();

                result2 = true;
                foreach (var token in myTokens)
                {
                    var parent = token.Parent;

                    string parentCondition = "parentNode.RawKind == syntaxKind.Value(\"IdentifierName\")";
                    var parentExpression = DynamicExpression.ParseLambda(new[] { nodeParam, syntaxKindParam }, typeof(bool), parentCondition);
                    var thisResult = (bool)parentExpression.Compile().DynamicInvoke(parent, syntaxKind);
                    if (!thisResult)
                    {
                        result2 = false;
                        break;
                    }
                }
            }

            {
                // AncestorNodeForEachToken
                var tokenSelector = "tokens.Where(Text == \"Parse\" && GetNextToken(false, false, false, false).Text == \"(\")";
                var tokensParam = Expression.Parameter(typeof(IEnumerable<SyntaxToken>), "tokens");
                var expression = DynamicExpression.ParseLambda(new[] { tokensParam }, typeof(IEnumerable<SyntaxToken>), tokenSelector);

                var myTokens = (IEnumerable<SyntaxToken>)expression.Compile().DynamicInvoke(tokens);

                var nodeParam = Expression.Parameter(typeof(SyntaxNode), "parentNode");
                var syntaxKindParam = Expression.Parameter(typeof(EnumHelper<SyntaxKind, ushort>), "syntaxKind");
                var syntaxKind = new EnumHelper<SyntaxKind, ushort>();

                result3 = true;
                foreach (var token in myTokens)
                {
                    var resultForThisToken = false;
                    var parent = token.Parent;
                    while (parent != null)
                    {
                        string parentCondition = "parentNode.RawKind == syntaxKind.Value(\"SimpleAssignmentExpression\")";
                        var parentExpression = DynamicExpression.ParseLambda(new[] { nodeParam, syntaxKindParam }, typeof(bool), parentCondition);
                        var thisResult = (bool)parentExpression.Compile().DynamicInvoke(parent, syntaxKind);

                        // Process the result
                        if (thisResult) // Found it!
                        {
                            resultForThisToken = true;
                            break;
                        }

                        parent = parent.Parent;
                    }
                    if (resultForThisToken == false)
                    {
                        result3 = false;
                        break;
                    }
                }
            }


            return Task.FromResult(result1 && result2 && result3);
        }
    }
}
