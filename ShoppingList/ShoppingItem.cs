namespace ShoppingList{
public class ShoppingItem
{
    public Product.Product Product { get; set; }
    public int Quantity { get; set; }
    public bool IsAdded { get; set; }

    public ShoppingItem(Product.Product product, int quantity)
    {
        Product = product;
        Quantity = quantity;
        IsAdded = false;
    }
}
}