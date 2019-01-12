using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.FindSymbols;

namespace CodingTrainer.CSharpRunner.Assessment.Methods
{
    class ContainsIntDotParseAssessment : AssessmentByInspectionBase
    {
        protected override async Task<bool> DoAssessmentAsync()
        {
            // NOT COMPLETE, NOT WORKING!

            var tree = Compilation.CompilationObject.SyntaxTrees.Single();
            var model = Compilation.CompilationObject.GetSemanticModel(tree);

            ISymbol parse = Compilation.CompilationObject.GetTypeByMetadataName("System.Int32.Parse");

            
            Solution s = null;
            var result = await SymbolFinder.FindReferencesAsync(parse, s);

            return false;
        }
    }
}
