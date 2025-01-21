namespace Recipe{
public class IngredientList{
    public List<Ingredient> Ingredients { get; set; }

    public IngredientList()
    {
        Ingredients = new List<Ingredient>();
    }
    public IEnumerator<Ingredient> GetEnumerator() {

        return Ingredients.GetEnumerator();
    }
    public void Add(Product.Product product, double amount)
    {
        Ingredients.Add(new Ingredient(product, amount));
    }

    public void Remove(Product.Product product)
    {
        Ingredients.RemoveAll(i => i.Product == product);
    }

    public double GetTotalProtein()
    {
        double totalProtein = 0;
        foreach (var ingredient in Ingredients)
        {
            totalProtein += ingredient.Product.Protein * ingredient.Amount;
        }
        return totalProtein;
    }

    public double GetTotalFat()
    {
        double totalFat = 0;
        foreach (var ingredient in Ingredients)
        {
            totalFat += ingredient.Product.Fat * ingredient.Amount;
        }
        return totalFat;
    }

    public double GetTotalCarbohydrates()
    {
        double totalCarbohydrates = 0;
        foreach (var ingredient in Ingredients)
        {
            totalCarbohydrates += ingredient.Product.Carbohydrates * ingredient.Amount;
        }
        return totalCarbohydrates;
    }
}
}