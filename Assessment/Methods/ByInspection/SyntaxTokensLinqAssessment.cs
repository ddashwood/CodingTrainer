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
    public class SyntaxTokensLinqAssessment : AssessmentByInspectionBase
    {
        public string Condition { get; set; }

        protected override Task<bool> AssessCompilationAsync(Compilation compilation)
        {
            var tree = compilation.SyntaxTrees.Single();
            var tokens = tree.GetRoot().DescendantTokens(c => true);

            bool result;

            var tokensParam = Expression.Parameter(typeof(IEnumerable<SyntaxToken>), "tokens");
            var expression = DynamicExpression.ParseLambda(new[] { tokensParam }, typeof(bool), Condition);

            result = (bool)expression.Compile().DynamicInvoke(tokens);

            return Task.FromResult(result);
        }
    }
}
