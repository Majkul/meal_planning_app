namespace ShoppingList.Decorators{
public class MarkAsAddedDecorator : ShoppingListDecorator
{
    public MarkAsAddedDecorator(IShoppingList shoppingList) : base(shoppingList) { }
    
    public override void MarkAsAdded(Product.Product product)
    {
        base.MarkAsAdded(product);
        Console.WriteLine($"Produkt '{product.Name}' został oznaczony jako dodany do koszyka.");
    }
    public override void DisplayProducts()
    {
        Console.WriteLine("\nLista zakupów według kategorii:");
        var categorizedItems = shoppingList.GetItems()
            .GroupBy(i => i.Product.Category)
            .ToDictionary(g => g.Key, g => g.ToList());

        if (categorizedItems.Count == 0)
        {
            Console.WriteLine("Brak produktów na liście.");
        }
        else
        {
            foreach (var category in categorizedItems)
            {
                Console.WriteLine($"\nKategoria: {category.Key}");
                foreach (var item in category.Value)
                {
                    Console.WriteLine($"  {item.Product.Name} - {item.Quantity} {item.Product.Unit} (Dodany: {(item.IsAdded ? "Tak" : "Nie")})");
                }
            }
        }
    }
}
}