using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
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
            var tokens = tree.GetRoot().DescendantTokens(c => true); // Can find Parse in here, don't know if it's a method call...

            var lastLineParam = Expression.Parameter(typeof(IEnumerable<SyntaxToken>), "tokens");
            var expression = DynamicExpression.ParseLambda(new[] { lastLineParam }, typeof(bool),
                  "(tokens.Where(Text == \"Parse\").Where(GetNextToken(false, false, false, false).Text == \"(\").Count()) == 2");

            var result = (bool)expression.Compile().DynamicInvoke(tokens);
            return Task.FromResult(result);
        }
    }
}
