// We've got the itemsSymbol from the semantic model
// Now we need to find the item in the syntax tree which declares the symbol
var itemsPropSyntax = ((PropertyDeclarationSyntax)itemsSymbol.DeclaringSyntaxReferences[0].GetSyntax());
var useField = false;

if (itemsPropSyntax.AccessorList.Accessors.Any(a => a.Body != null || a.ExpressionBody != null))
{
    // Not an auto property
    // That means there should be a field to hold the data
    useField = true;
    var itemsFieldSymbol = ModelHelper.GetSymbolInClass("Basket", "items");
    VariableDeclaratorSyntax itemsFieldSyntax = null;

    if (itemsFieldSymbol != null)
    {
        itemsFieldSyntax = itemsFieldSymbol.DeclaringSyntaxReferences[0].GetSyntax() as VariableDeclaratorSyntax;
    }

    if (itemsFieldSyntax == null)
    {
        WriteToConsole("You do not have an \"items\" field in your Basket class\r\n\r\n");
        WriteToConsole("If you write a body in the Getter of the Items property, you will need a field to store the data\r\n");
        return false;
    }

    var assigned = itemsFieldSyntax.Initializer != null;
    if (assigned) return true;
}
else
{
    var assigned = itemsPropSyntax.Initializer != null;
    if (assigned) return true;
}


// The field/property is not assigned in the declaration, so check the constructor

string itemsText = useField ? "\"items\" field" : "\"Items\" property";
string itemsName = useField ? "items" : "Items";

var basketConstructors = ModelHelper.GetClassConstructors("Basket");
if (basketConstructors.Length == 1 && basketConstructors[0].DeclaringSyntaxReferences.Length == 0)
{
    // Auto-generated constructors have no declaring syntax references - this will happen if the
    // user hasn't created their own constructor
    WriteToConsole("The " + itemsText + " has not been initialised.\r\n\r\n");
    WriteToConsole("You can not put anything into a list until you've created the list, so use the \r\n");
    WriteToConsole("\"new\" keyword to create a list. You might want to do this inside a constructor");

    return false;
}
foreach (var basketConstructor in basketConstructors)
{
    if (!ModelHelper.MethodAssignsVariable(basketConstructor, itemsName))
    {
        WriteToConsole("The " + itemsText + "has not been initialised.\r\n\r\n");
        WriteToConsole("You can not put anything into a list until you've created the list, so use the \r\n");
        WriteToConsole("\"new\" keyword to create a list inside the constructor");

        return false;
    }
}

return true;