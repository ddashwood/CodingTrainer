using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
                var classSyntax = Nodes.OfType<TypeDeclarationSyntax>().SingleOrDefault(t => t.Identifier.Text == className);
                if (classSyntax == null) return null;

                var classModel = SemanticModel.GetDeclaredSymbol(classSyntax) as INamedTypeSymbol;
                if (classModel == null) return null;

                var member = classModel.GetMembers().SingleOrDefault(m => m.Name == memberName);

                return member;

            }


            public ImmutableArray<IMethodSymbol> GetClassConstructors(string className)
            {
                var classSyntax = Nodes.OfType<TypeDeclarationSyntax>().SingleOrDefault(t => t.Identifier.Text == className);
                if (classSyntax == null) return new ImmutableArray<IMethodSymbol>();

                var classModel = SemanticModel.GetDeclaredSymbol(classSyntax) as INamedTypeSymbol;
                if (classModel == null) return new ImmutableArray<IMethodSymbol>();
                
                return classModel.Constructors;
            }

            public bool MethodAssignsVariable(IMethodSymbol method, string variableName)
            {
                var syntax = method.DeclaringSyntaxReferences[0].GetSyntax();
                var assignments = syntax.DescendantNodes().Where(n => n.IsKind(SyntaxKind.SimpleAssignmentExpression));

                return assignments.Any(a => ((IdentifierNameSyntax)((AssignmentExpressionSyntax)a).Left).Identifier.Text == variableName);
            }
        }
    }
}
