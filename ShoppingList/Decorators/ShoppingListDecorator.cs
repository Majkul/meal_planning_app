namespace ShoppingList.Decorators{
public abstract class ShoppingListDecorator : IShoppingList
{
    protected IShoppingList shoppingList;

    public ShoppingListDecorator(IShoppingList shoppingList)
    {
        this.shoppingList = shoppingList;
    }

    public virtual void AddProduct(Product.Product product, int quantity)
    {
        shoppingList.AddProduct(product, quantity);
    }
    public virtual void GenerateFromRecipe(Recipe.Recipe recipe)
    {
        shoppingList.GenerateFromRecipe(recipe);
    }

    public virtual void RemoveProduct(Product.Product product)
    {
        shoppingList.RemoveProduct(product);
    }
    public virtual void MarkAsAdded(Product.Product product)
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
}