var itemsSymbol = ModelHelper.GetSymbolInClass("Basket", "Items");
if (itemsSymbol == null)
{
	WriteToConsole("You do not have an Items property in your Basket class\r\n");
	return false;
}

if (itemsSymbol.Kind != SymbolKind.Property)
{
    WriteToConsole("The Items element in your Basket class is not a property.\r\n\r\nMake sure it is followed by curly brackets, with a Getter in the brackets\r\n");
    return false;
}

var propertySymbol = itemsSymbol as IPropertySymbol;
var correctType = false;

if (propertySymbol.Type.Name == "IEnumerable" || propertySymbol.Type.Name == "List")
{
    var type = propertySymbol.Type as INamedTypeSymbol;
	if (type != null && type.TypeArguments.Length > 0 && type.TypeArguments[0].Name == "BasketItem")
    {
        correctType = true;
    }
}


if (!correctType)
{
    WriteToConsole("The data type of the Items property is not correct.\r\n\r\nThe Items property needs to return a List<BasketItem>, so\r\ndeclare it to either be of type List<BasketItem> or IEnumerable<BasketItem>\r\n");
    return false;
}

return true;
