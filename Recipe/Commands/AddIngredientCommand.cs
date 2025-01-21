namespace Recipe.Commands{
public class AddIngredientCommand : ICommand
{
    private readonly RecipeBuilder recipeBuilder;
    private readonly Product.Product product;
    private readonly double amount;

    public AddIngredientCommand(RecipeBuilder recipeBuilder, Product.Product product, double amount)
    {
        this.recipeBuilder = recipeBuilder;
        this.product = product;
        this.amount = amount;
    }

    public void Execute()
    {
        recipeBuilder.AddIngredient(product, amount);
    }

    public void Undo()
    {
        recipeBuilder.RemoveIngredient(product);
    }
}
}