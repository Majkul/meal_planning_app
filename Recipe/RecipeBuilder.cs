namespace Recipe{
public class RecipeBuilder {
    private Recipe _recipe = new Recipe();
    public RecipeBuilder AddName(string name) {
        _recipe.Name = name;
        return this;
    }

    public RecipeBuilder AddIngredient(Product.Product product, double amount) {
        _recipe.Ingredients.Add(product, amount);
        return this;
    }

    public RecipeBuilder RemoveIngredient(Product.Product product) {
        _recipe.Ingredients.Remove(product);
        return this;
    }

    public RecipeBuilder AddInstructionStep(string instruction) {
        _recipe.Instructions.Add(instruction);
        return this;
    }

    public RecipeBuilder AddTotalTime(TimeSpan time) {
        _recipe.TotalTime = time;
        return this;
    }

    private RecipeBuilder TotalProtein() {
        _recipe.Protein = _recipe.Ingredients.GetTotalProtein();
        return this;
    }

    private RecipeBuilder TotalFat() {
        _recipe.Fat = _recipe.Ingredients.GetTotalFat();
        return this;
    }

    private RecipeBuilder TotalCarbohydrates() {
        _recipe.Carbohydrates = _recipe.Ingredients.GetTotalCarbohydrates();
        return this;
    }

    private RecipeBuilder TotalCalories() {
        _recipe.Calories = (int)(_recipe.Protein * 4 + _recipe.Fat * 9 + _recipe.Carbohydrates * 4);
        return this;
    }

    public RecipeBuilder CalculateNutrition() {
        TotalProtein();
        TotalFat();
        TotalCarbohydrates();
        TotalCalories();
        return this;
    }

    public Recipe Build() {
        return _recipe;
    }
}
}