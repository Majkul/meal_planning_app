using System;
using System.Collections.Generic;
using System.Linq;

public class Product
{
    public enum Category
    {
        Fruit,
        Vegetable,
        Meat,
        Dairy,
        Grain,
        Other
    }

    public string Name { get; set; }
    public Category CategoryType { get; set; }
    public string Unit { get; set; }
    public double Protein { get; set; }
    public double Fat { get; set; }
    public double Carbohydrates { get; set; }

    public Product(string name, string unit, Category category, double protein, double fat, double carbohydrates)
    {
        Name = name;
        Unit = unit;
        CategoryType = category;
        Protein = protein;
        Fat = fat;
        Carbohydrates = carbohydrates;
    }
}
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
}

public class ShoppingList : IShoppingList
{
    private List<ShoppingItem> items = new();

    public void AddProduct(Product product, int quantity)
    {
        var existingItem = items.FirstOrDefault(i => i.Product == product);
        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
        }
        else
        {
            items.Add(new ShoppingItem(product, quantity));
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
        var existingItem = items.FirstOrDefault(i => i.Product == product);
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
                Console.WriteLine($"{item.Product.Name} - {item.Quantity} {item.Product.Unit} (Dodany: {(item.IsAdded ? "Tak" : "Nie")})");
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
            .GroupBy(i => i.Product.CategoryType)
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
}

class Program
{
    static void Main(string[] args)
    {
        IShoppingList shoppingList = new ShoppingList();
        shoppingList = new CategorizedDisplayDecorator(shoppingList);
        shoppingList = new MarkAsAddedDecorator(shoppingList);

        var apple = new Product("Jabłko", "szt.", Product.Category.Fruit, 0.3, 0.2, 14);
        var bread = new Product("Chleb", "szt.", Product.Category.Grain, 8, 1.2, 50);

        shoppingList.AddProduct(apple, 3);
        shoppingList.AddProduct(bread, 1);

        shoppingList.DisplayProducts();

        shoppingList.MarkAsAdded(apple);


        shoppingList.DisplayProducts();
    }
}
