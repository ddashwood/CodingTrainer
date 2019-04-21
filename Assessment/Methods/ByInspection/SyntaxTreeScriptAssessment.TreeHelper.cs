using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CodingTrainer.CSharpRunner.Assessment.Methods.ByInspection
{
    partial class SyntaxTreeScriptAssessment
    {
        public class TreeHelper
        {
            public SyntaxTree Tree { get; }

            public TreeHelper(SyntaxTree tree)
            {
                Tree = tree;
            }

            public bool NodesOnRightOfEquals(IEnumerable<SyntaxNode> nodes)
            {
                bool result = true;

                foreach (var node in nodes)
                {
                    var resultForThisToken = false;
                    var currentNode = node;
                    if (ProcessParentNodes(currentNode, node, resultForThisToken, ref result)) break;
                }


                return result;
            }

            private static bool ProcessParentNodes(SyntaxNode currentNode, SyntaxNode node, bool resultForThisToken,
                ref bool result)
            {
                while (currentNode != null)
                {
                    var isVariableDeclaredButAssignedSeparatly = currentNode.Kind() == SyntaxKind.SimpleAssignmentExpression;
                    var isVariableDeclaredAndAssigned = currentNode.Kind() == SyntaxKind.EqualsValueClause;
                    if (isVariableDeclaredButAssignedSeparatly)
                    {
                        var parentAssignment = (AssignmentExpressionSyntax) currentNode;
                        if (parentAssignment.Right.Contains(node))
                        {
                            resultForThisToken = true;
                            break;
                        }
                    }
                    else if (isVariableDeclaredAndAssigned)
                    {
                        resultForThisToken = true;
                        break;
                    }

                    currentNode = currentNode.Parent;
                }

                if (resultForThisToken == false)
                {
                    result = false;
                    return true;
                }

                return false;
            }

            public bool TokensHaveSpecifiedAncestorNode(Func<SyntaxToken, bool> tokenSelector, Func<SyntaxNode, bool> parentCondition)
            {
                var tokens = Tree.GetRoot().DescendantTokens(c => true).Where(tokenSelector);

                foreach (var token in tokens)
                {
                    var isParentFound = IsParentFound(parentCondition, token);
                    if (isParentFound == false) return false;
                }


                return true;
            }

            private static bool IsParentFound(Func<SyntaxNode, bool> parentCondition, object token)
            {
                var parent = token.Parent;
                while (parent != null)
                {
                    var isParentFound = parentCondition(parent);
                    if (isParentFound) return true;
                    parent = parent.Parent;
                }

                return false;
            }
        }
    }
}
