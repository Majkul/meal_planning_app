namespace ShoppingList{
public class ShoppingList : IShoppingList
{
    private List<ShoppingItem> items = new();

    public void AddProduct(Product.Product product, int quantity)
    {
        var existingItem = items.FirstOrDefault(i => i.Product.Name == product.Name);
        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
        }
        else
        {
            items.Add(new ShoppingItem(product, quantity));
        }
    }
    public void GenerateFromRecipe(Recipe.Recipe recipe)
    {
        foreach (var ingredient in recipe.Ingredients.Ingredients)
        {
            AddProduct(ingredient.Product, (int)ingredient.Amount);
        }
    }
    public void RemoveProduct(Product.Product product)
    {
        var existingItem = items.FirstOrDefault(i => i.Product == product);
        if (existingItem != null)
        {
            items.Remove(existingItem);
        }
    }

    public void MarkAsAdded(Product.Product product)
    {
        var existingItem = items.FirstOrDefault(i => i.Product.Name == product.Name);
        if (existingItem != null)
        {
            existingItem.IsAdded = true;
        }
    }

    public void DisplayProducts()
    {
        Console.WriteLine("\nLista zakupów:");
        if (items.Count == 0)
        {
            Console.WriteLine("Brak produktów na liście.");
        }
        else
        {
            foreach (var item in items)
            {
                Console.WriteLine($"{item.Product.Name} - {item.Quantity} {item.Product.Unit}");
            }
        }
    }

    public List<ShoppingItem> GetItems()
    {
        return items;
    }
}
}