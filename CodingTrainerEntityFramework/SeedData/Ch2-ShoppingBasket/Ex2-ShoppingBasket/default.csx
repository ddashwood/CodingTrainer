class Basket
{
    // Add an Items property

    // Add a TotalPrice property

    public void AddItem(int itemId, string description, decimal price)
    {
        // See if this item is already in the basket
        var item = Items.SingleOrDefault(i => i.ItemId == itemId);

        // Add the item
    }

    // Override the ToString() method
}

class BasketItem
{
    public int ItemId { get; }
    public string Description { get; }
    public decimal UnitPrice { get; }
    public int Quantity { get; private set; }

    public BasketItem(int itemId, string description, decimal price)
    {
        ItemId = itemId;
        Description = description;
        UnitPrice = price;
        // Quantity = ?????;
    }

    public void IncrementQty()
    {
        Quantity++;
    }

    public decimal TotalPrice
    {
        get
        {
            // Put the correct calculation in here
            return 0;
        }
    }

    public override string ToString()
    {
        return $"{Quantity} x {Description} @ {UnitPrice:c2} each = {TotalPrice:c2}{Environment.NewLine}";
    }
}
