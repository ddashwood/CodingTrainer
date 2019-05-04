using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using DynamicExpression = System.Linq.Dynamic.DynamicExpression;

namespace CodingTrainer.CSharpRunner.Assessment.Methods.ByRunning
{
    public class LinqAssessment : AssessmentByRunningBase
    {
        [Required]
        public string Condition { get; set; }

        protected override bool CheckResult(string consoleOut)
        {
            var lastLineParam = Expression.Parameter(typeof(string), "output");
            var expression = DynamicExpression.ParseLambda(new[] { lastLineParam }, typeof(bool), Condition);

            var result = (bool)expression.Compile().DynamicInvoke(consoleOut);
            return result;
        }
    }
}
