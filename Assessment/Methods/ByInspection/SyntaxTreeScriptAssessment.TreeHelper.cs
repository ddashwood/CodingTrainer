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
                    while (currentNode != null)
                    {
                        // Handle the case where variable already declared, now being assigned to:
                        //   int a;
                        //   a = <this node>;
                        if (currentNode.Kind() == SyntaxKind.SimpleAssignmentExpression)
                        {
                            var parentAssignment = (AssignmentExpressionSyntax)currentNode;
                            if (parentAssignment.Right.Contains(node))
                            {
                                resultForThisToken = true;
                                break;
                            }
                        }
                        // Handle the case where variable is declared and initialised:
                        //    int a = <this node>;
                        else if (currentNode.Kind() == SyntaxKind.EqualsValueClause)
                        {
                            resultForThisToken = true;
                            break;
                        }

                        currentNode = currentNode.Parent;
                    }
                    if (resultForThisToken == false)
                    {
                        result = false;
                        break;
                    }
                }


                return result;
            }

            public bool TokensHaveSpecifiedAncestorNode(Func<SyntaxToken, bool> tokenSelector, Func<SyntaxNode, bool> parentCondition)
            {
                bool result = true;
                var tokens = Tree.GetRoot().DescendantTokens(c => true).Where(tokenSelector);

                foreach (var token in tokens)
                {
                    var resultForThisToken = false;
                    var parent = token.Parent;
                    while (parent != null)
                    {
                        var thisResult = parentCondition(parent);
                        if (thisResult) // Found it!
                        {
                            resultForThisToken = true;
                            break;
                        }

                        parent = parent.Parent;
                    }
                    if (resultForThisToken == false)
                    {
                        result = false;
                        break;
                    }
                }


                return result;
            }
        }
    }
}
