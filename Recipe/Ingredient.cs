namespace Recipe{
public class Ingredient
    {
        public Product.Product Product { get; set; }
        public double Amount { get; set; }

        public Ingredient(Product.Product product, double amount)
        {
            Product = product;
            Amount = amount;
        }
    } 
}