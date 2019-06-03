using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static System.Math;
using static System.Console;

static void Main()
{
    WriteLine("Adding items to the shopping basket.");
    WriteLine();
    var basket = new Basket();
    bool cont;

    do
    {
        Write("Enter the item ID: ");
        string sId = ReadLine();
        if (int.TryParse(sId, out int id))
        {
            string description;
            decimal price;
            if (basket.Items == null)
            {
                throw new InvalidOperationException("Basket object's Items property is null");
            }
            var item = basket.Items.SingleOrDefault(i => i.ItemId == id);
            if (item == null)
            {
                Write("Enter item description: ");
                description = ReadLine();

                Write("Enter item price: ");
                string sPrice = ReadLine();
                bool success;
                do
                {
                    success = decimal.TryParse(sPrice, out price);
                    if (!success)
                    {
                        Write("That is not a valid price. Enter item price: ");
                        sPrice = ReadLine();
                    }
                } while (!success);
            }
            else
            {
                description = item.Description;
                price = item.UnitPrice;
            }

            basket.AddItem(id, description, price);
            WriteLine($"Item \"{description}\" added to basket");
        }
        else
        {
            WriteLine("Item ID must be a whole number");
        }

        Write("Do you want to add another item? Enter Y to continue, anything else stops: ");
        string sCont = ReadLine();
        cont = (sCont == "Y" || sCont == "y");
    } while (cont);

    WriteLine();
    WriteLine("Here are the contents of your basket:");
    WriteLine();
    WriteLine(basket.ToString());
}
