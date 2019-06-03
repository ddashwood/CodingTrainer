var totalPriceSymbol = ModelHelper.GetSymbolInClass("Basket", "TotalPrice");
if (totalPriceSymbol == null)
{
	WriteToConsole("You do not have a TotalPrice property in your Basket class\r\n");
	return false;
}

if (totalPriceSymbol.Kind != SymbolKind.Property)
{
    WriteToConsole("The TotalPrice element in your Basket class is not a property.\r\n\r\nMake sure it is followed by curly brackets, with a Getter in the brackets\r\n");
    return false;
}

PropertyDeclarationSyntax totalPricePropSyntax = ((PropertyDeclarationSyntax)totalPriceSymbol.DeclaringSyntaxReferences[0].GetSyntax());
var totalPriceGetters = totalPricePropSyntax.AccessorList.Accessors.Where(a => a.IsKind(SyntaxKind.GetAccessorDeclaration));
var totalPriceSetters = totalPricePropSyntax.AccessorList.Accessors.Where(a => a.IsKind(SyntaxKind.SetAccessorDeclaration));

if (totalPriceSetters.Any())
{
    WriteToConsole("The TotalPrice property in your Basket class should be a calculated property.\r\n\r\nThat means it should not have a Setter. The Getter should calculate the total price\r\n");
    return false;
}

if (!totalPriceGetters.Any())
{
    WriteToConsole("The TotalPrice property in your Basket class should be a calculated property.\r\n\r\nThat means it should have a Getter which should calculate the total price\r\n");
    return false;
}

var totalPriceGetter = totalPriceGetters.First();
if (totalPriceGetter.Body == null && totalPriceGetter.ExpressionBody == null) // Auto-property
{
    WriteToConsole("The TotalPrice property in your Basket class should be a calculated property.\r\n\r\nThat means it should have a Getter which must have a body that calculates the\r\ntotal price. Make sure the word \"get\" is followed by curly brackets, and not by a semi-colon");
    return false;
}

return true;
