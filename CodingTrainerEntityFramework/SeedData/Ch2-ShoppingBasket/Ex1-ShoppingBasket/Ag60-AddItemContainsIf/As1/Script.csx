var addItemSymbol = ModelHelper.GetSymbolInClass("Basket", "AddItem");
if (addItemSymbol == null)
{
	WriteToConsole("You do not have an AddItem() method in your Basket class\r\n");
	return false;
}

if (addItemSymbol.Kind != SymbolKind.Method)
{
    WriteToConsole("The AddItem element in your Basket class is not a method.");
    return false;
}

MethodDeclarationSyntax addItemSyntax = ((MethodDeclarationSyntax)addItemSymbol.DeclaringSyntaxReferences[0].GetSyntax());
IEnumerable<SyntaxNode> addItemsIfNodes = addItemSyntax.DescendantNodes().Where(n => n.IsKind(SyntaxKind.IfStatement));
if (!addItemsIfNodes.Any())
{
    WriteToConsole("You will need to use an \"if\" statement in the AddItem() method, to handle two different scenarios.\r\nIn one scenario, this item ID does not exist in the basket yet.\r\nIn the other scenario, it does exist and needs to have its quantity increased.");
    return false;
}
IEnumerable<SyntaxNode> addItemsSingleOrDefaultNodes = addItemSyntax.DescendantNodes().Where(n => n.GetText().ToString() == "SingleOrDefault");
if (!addItemsSingleOrDefaultNodes.Any())
{
    WriteToConsole("You will need to call Items.SingleOrDefault() before the \"if\" statement to attempt\r\nto retrieve the item with the correct ID from the basket");
    return false;
}
if (addItemsSingleOrDefaultNodes.First().FullSpan.Start > addItemsIfNodes.First().FullSpan.Start) // SingleOrDefault comes after If
{
    WriteToConsole("You will need to call Items.SingleOrDefault() before (not after!) the \"if\" statement to attempt\r\nto retrieve the item with the correct ID from the basket");
    return false;
}

return true;
