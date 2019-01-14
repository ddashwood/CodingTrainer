using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTrainer.CSharpRunner.Assessment.Methods
{
    class SyntaxTreeScriptingAssessment:AssessmentByInspectionBase
    {
        protected override Task<bool> DoAssessmentAsync()
        {
            ScriptOptions options = ScriptOptions.Default;
            CSharpScript.EvaluateAsync("", options);

            return Task.FromResult(false);
        }
    }
}
