using RecipeNamespace;
namespace ShoppingListNamespace{
    public class ShoppingItem
{
    public Product Product { get; set; }
    public int Quantity { get; set; }
    public bool IsAdded { get; set; }

    public ShoppingItem(Product product, int quantity)
    {
        Product = product;
        Quantity = quantity;
        IsAdded = false;
    }
}

public interface IShoppingList
{
    void AddProduct(Product product, int quantity);
    void RemoveProduct(Product product);
    void MarkAsAdded(Product product);
    void DisplayProducts();
    List<ShoppingItem> GetItems();
    void GenerateFromRecipe(Recipe recipe);
}

public class ShoppingList : IShoppingList
{
    private List<ShoppingItem> items = new();

    public void AddProduct(Product product, int quantity)
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
    public void GenerateFromRecipe(Recipe recipe)
    {
        foreach (var ingredient in recipe.Ingredients.Ingredients)
        {
            AddProduct(ingredient.Product, (int)ingredient.Amount);
        }
    }
    public void RemoveProduct(Product product)
    {
        var existingItem = items.FirstOrDefault(i => i.Product == product);
        if (existingItem != null)
        {
            items.Remove(existingItem);
        }
    }

    public void MarkAsAdded(Product product)
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

public abstract class ShoppingListDecorator : IShoppingList
{
    protected IShoppingList shoppingList;

    public ShoppingListDecorator(IShoppingList shoppingList)
    {
        this.shoppingList = shoppingList;
    }

    public virtual void AddProduct(Product product, int quantity)
    {
        shoppingList.AddProduct(product, quantity);
    }
    public virtual void GenerateFromRecipe(Recipe recipe)
    {
        shoppingList.GenerateFromRecipe(recipe);
    }

    public virtual void RemoveProduct(Product product)
    {
        shoppingList.RemoveProduct(product);
    }

    public virtual void MarkAsAdded(Product product)
    {
        shoppingList.MarkAsAdded(product);
    }

    public virtual void DisplayProducts()
    {
        shoppingList.DisplayProducts();
    }

    public virtual List<ShoppingItem> GetItems()
    {
        return shoppingList.GetItems();
    }
}

public class CategorizedDisplayDecorator : ShoppingListDecorator
{
    public CategorizedDisplayDecorator(IShoppingList shoppingList) : base(shoppingList) { }

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

public class MarkAsAddedDecorator : ShoppingListDecorator
{
    public MarkAsAddedDecorator(IShoppingList shoppingList) : base(shoppingList) { }
    
    public override void MarkAsAdded(Product product)
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