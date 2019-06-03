var toStringSymbol = ModelHelper.GetSymbolInClass("Basket", "ToString");
if (toStringSymbol == null)
{
    WriteToConsole("You must override the ToString() method\r\n");
    return false;
}

MethodDeclarationSyntax toStringSyntax = ((MethodDeclarationSyntax)toStringSymbol.DeclaringSyntaxReferences[0].GetSyntax());
if (!toStringSyntax.Modifiers.Any(m => m.IsKind(SyntaxKind.OverrideKeyword)))
{
    WriteToConsole("The ToString() method must be marked with the \"override\" modifier\r\n");
    return false;
}


// Get foreach nodes
IEnumerable<SyntaxNode> toStringForEachNodes = toStringSyntax.DescendantNodes().Where(n => n.IsKind(SyntaxKind.ForEachStatement));
if (!toStringForEachNodes.Any())
{
    if (toStringSyntax.DescendantNodes().Any(
        n => n.IsKind(SyntaxKind.ForStatement) ||
             n.IsKind(SyntaxKind.DoStatement) ||
             n.IsKind(SyntaxKind.WhileStatement)))
    {
        WriteToConsole("Your ToString() method contains the wrong kind of loop.\r\n\r\nUse a \"foreach\" loop to iterate through a list.\r\n");
        return false;
    }

    WriteToConsole("You will need a loop in the ToString() method to go through each basket item.\r\n");
    return false;
}

var toStringForEachNode = toStringForEachNodes.First() as ForEachStatementSyntax;

if (toStringForEachNode.Expression.GetText().ToString() != "Items")
{
    WriteToConsole("Your ToString() method has a \"foreach\" statement, but it might not be looping through each entry in Items\r\n");
    return false;
}

return true;
