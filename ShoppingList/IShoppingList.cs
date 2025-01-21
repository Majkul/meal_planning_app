namespace ShoppingList{
public interface IShoppingList
{
    void AddProduct(Product.Product product, int quantity);
    void RemoveProduct(Product.Product product);
    void MarkAsAdded(Product.Product product);
    void DisplayProducts();
    List<ShoppingItem> GetItems();
    void GenerateFromRecipe(Recipe.Recipe recipe);
}
}