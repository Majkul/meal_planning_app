namespace Recipe.Commands{
public class DeleteIngredientCommand : ICommand
{
    private readonly RecipeBuilder recipeBuilder;
    private readonly Product.Product product;
    private Ingredient removedIngredient;

    public DeleteIngredientCommand(RecipeBuilder recipeBuilder, Product.Product product)
    {
        this.recipeBuilder = recipeBuilder;
        this.product = product;
    }

    public void Execute()
    {
        removedIngredient = recipeBuilder.Build().Ingredients.Ingredients.FirstOrDefault(i => i.Product == product);
        recipeBuilder.RemoveIngredient(product);
    }

    public void Undo()
    {
        if (removedIngredient != null)
        {
            recipeBuilder.AddIngredient(product, removedIngredient.Amount);
        }
    }
}
}