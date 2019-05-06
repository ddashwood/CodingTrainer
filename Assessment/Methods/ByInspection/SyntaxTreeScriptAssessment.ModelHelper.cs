using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTrainer.CSharpRunner.Assessment.Methods.ByInspection
{
    partial class SyntaxTreeScriptAssessment
    {
        public class ModelHelper
        {
            public SemanticModel SemanticModel { get; }
            public IEnumerable<SyntaxNode> Nodes { get; }

            public ModelHelper(SemanticModel semanticModel, IEnumerable<SyntaxNode> nodes)
            {
                SemanticModel = semanticModel;
                Nodes = nodes;
            }

            public ISymbol GetSymbolInClass(string className, string memberName)
            {
                var classSyntax = Nodes.OfType<Microsoft.CodeAnalysis.CSharp.Syntax.TypeDeclarationSyntax>().SingleOrDefault(t => t.Identifier.Text == className);
                if (classSyntax == null) return null;

                var classModel = SemanticModel.GetDeclaredSymbol(classSyntax) as INamedTypeSymbol;
                if (classModel == null) return null;

                var member = classModel.GetMembers().SingleOrDefault(m => m.Name == memberName);
                return member;
            }
        }
    }
}
