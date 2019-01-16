using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace CodingTrainer.CSharpRunner.Assessment.Methods
{
    public class SyntaxTreeScriptAssessment : AssessmentByInspectionBase
    {
        public class Globals
        {
            public SyntaxTree Tree { get; set; }
            public Action<string> WriteToConsole { get; set; }
        }


        protected async override Task<bool> AssessCompilationAsync(Compilation compilation)
        {
            var tree = compilation.SyntaxTrees.Single();

            ScriptOptions options = ScriptOptions.Default
                .AddReferences(
                    Assembly.GetAssembly(typeof(SyntaxTree)),
                    Assembly.Load("System.Runtime, Version=4.0.20.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"),
                    Assembly.Load("System.Threading.Tasks, Version = 4.0.10.0, Culture = neutral, PublicKeyToken = b03f5f7f11d50a3a")
                ).AddImports("Microsoft.CodeAnalysis","System.Linq");

            var globals = new Globals { Tree = tree, WriteToConsole = WriteToConsole };
            string script = "WriteToConsole(\"Hello 1\\n\"); string fromPrevious=\"Hello from previous\\n\"; Tree.GetRoot().DescendantTokens(n => true).Count(t => t.Text == \"Parse\") == 3";
            var state = await CSharpScript.RunAsync<bool>(script, options, globals);

            state = await state.ContinueWithAsync<bool>("WriteToConsole(fromPrevious); true");

            return state.ReturnValue;
        }
    }
}
