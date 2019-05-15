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
    public partial class SyntaxTreeScriptAssessment : AssessmentByInspectionBase
    {
        public class Globals
        {
            public Compilation Compilation { get; }
            public SyntaxTree Tree { get; }
            public SemanticModel SemanticModel { get; }
            public IEnumerable<SyntaxNode> Nodes { get; }
            public Action<string> WriteToConsole { get; }
            public TreeHelper TreeHelper { get; }
            public ModelHelper ModelHelper { get; }

            public Globals(Compilation compilation, SyntaxTreeScriptAssessment owner)
            {
                Compilation = compilation;
                Tree = compilation.SyntaxTrees.Single();
                SemanticModel = compilation.GetSemanticModel(Tree);
                Nodes = Tree.GetRoot().DescendantNodes(n => true);
                WriteToConsole = owner.WriteToConsole;
                TreeHelper = new TreeHelper(Tree);
                ModelHelper = new ModelHelper(SemanticModel, Nodes);
            }
        }

        public string Script { get; set; }


        protected async override Task<bool> AssessCompilationAsync(Compilation compilation)
        {
            ScriptOptions options = ScriptOptions.Default
                .AddReferences(
                    Assembly.GetAssembly(typeof(SyntaxTree)),
                    Assembly.GetAssembly(typeof(CSharpExtensions)),
                    Assembly.Load("System.Runtime, Version=4.0.20.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"),
                    Assembly.Load("System.Threading.Tasks, Version = 4.0.10.0, Culture = neutral, PublicKeyToken = b03f5f7f11d50a3a"),
                    Assembly.Load("System.Text.Encoding, Version = 4.0.10.0, Culture = neutral, PublicKeyToken = b03f5f7f11d50a3a"),
                    Assembly.Load("System.Collections.Immutable, Version=1.2.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")
                ).AddImports("Microsoft.CodeAnalysis", "Microsoft.CodeAnalysis.CSharp", "Microsoft.CodeAnalysis.CSharp.Syntax", "System.Collections.Generic", "System.Linq");


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

                var globals = new Globals(compilation, this);
                state = await CSharpScript.RunAsync<bool>(Script, options, globals);
            }

            AssessmentBag.AssessCompilationAsync_State = state;
            return state.ReturnValue;
        }
    }
}
