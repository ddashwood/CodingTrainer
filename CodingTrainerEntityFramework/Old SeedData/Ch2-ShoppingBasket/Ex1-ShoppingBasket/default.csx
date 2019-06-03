class Basket
{
    private List<BasketItem> items = new List<BasketItem>();

    public IEnumerable<BasketItem> Items
    {
        get
        {
            return items.AsReadOnly();
        }
    }

    public decimal TotalPrice
    {
        get
        {
            return Items.Sum(i => i.TotalPrice);
        }
    }

    public void AddItem(int itemId, string description, decimal price)
    {
        var item = items.SingleOrDefault(i => i.ItemId == itemId);

        if (item == null)
        {
            items.Add(new BasketItem(itemId, description, price));
        }
        else
        {
            if (item.Description != description)
                throw new InvalidOperationException("Description does match the existing basket item with the same ID");
            if (item.UnitPrice != price)
                throw new InvalidOperationException("Price does match the existing basket item with the same ID");

            item.IncrementQty();
        }
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        foreach (var item in Items)
        {
            sb.Append(item);
        }
        sb.Append($"Total value in basket: {TotalPrice:c2}");

        return sb.ToString();
    }
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
        Quantity = 1;
    }

    public void IncrementQty()
    {
        Quantity++;
    }

    public decimal TotalPrice
    {
        get
        {
            return UnitPrice * Quantity;
        }
    }

    public override string ToString()
    {
        return $"{Quantity} x {Description} @ {UnitPrice:c2} each = {TotalPrice:c2}{Environment.NewLine}";
    }
}
