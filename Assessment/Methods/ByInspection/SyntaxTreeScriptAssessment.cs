using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace CodingTrainer.CSharpRunner.Assessment.Methods.ByInspection
{
    public class SyntaxTreeScriptAssessment : AssessmentByInspectionBase
    {
        public class Globals
        {
            public SyntaxTree Tree { get; set; }
            public Action<string> WriteToConsole { get; set; }

            public Globals(SyntaxTree tree, SyntaxTreeScriptAssessment owner)
            {
                Tree = tree;
                WriteToConsole = owner.WriteToConsole;
            }
        }

        public string Script { get; set; }


        protected async override Task<bool> AssessCompilationAsync(Compilation compilation)
        {
            ScriptOptions options = ScriptOptions.Default
                .AddReferences(
                    Assembly.GetAssembly(typeof(SyntaxTree)),
                    Assembly.Load("System.Runtime, Version=4.0.20.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"),
                    Assembly.Load("System.Threading.Tasks, Version = 4.0.10.0, Culture = neutral, PublicKeyToken = b03f5f7f11d50a3a")
                ).AddImports("Microsoft.CodeAnalysis", "System.Linq");


            ScriptState<bool> state;
            if (((IDictionary<string, object>)AssessmentBag).ContainsKey("AssessCompilationAsync_State"))
            {
                // We found a State object, that means this is not the first script
                // Use the existing state so data can pass from one script to the next
                state = AssessmentBag.AssessCompilationAsync_State;
                state = await state.ContinueWithAsync<bool>(Script, options);
            }
            else
            {
                // This is the first script

                var tree = compilation.SyntaxTrees.Single();
                var globals = new Globals(tree, this);
                state = await CSharpScript.RunAsync<bool>(Script, options, globals);
            }

            AssessmentBag.AssessCompilationAsync_State = state;
            return state.ReturnValue;
        }
    }
}
