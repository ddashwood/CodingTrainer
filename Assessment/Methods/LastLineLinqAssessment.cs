using System;
using System.Linq;
using System.Linq.Expressions;
using DynamicExpression = System.Linq.Dynamic.DynamicExpression;

namespace CodingTrainer.CSharpRunner.Assessment.Methods
{
    public class LastLineLinqAssessment : AssessmentByRunningBase
    {
        protected override bool CheckResult(string consoleOut)
        {
            var lines = consoleOut.Split(new string[] { "\r", "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            var lastLine = lines[lines.Length - 1];

            var lastLineParam = Expression.Parameter(typeof(string), "output");
            var expression = DynamicExpression.ParseLambda(new[] { lastLineParam }, typeof(bool), ExpectedResult);

            var result = (bool)expression.Compile().DynamicInvoke(lastLine);
            return result;
        }
    }
}
