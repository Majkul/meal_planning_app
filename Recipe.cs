using System.Text;
namespace RecipeNamespace{
    public class IngredientList {
    public Dictionary<Product, double> Ingredients = new Dictionary<Product, double>();

    public void Add(Product product, double amount) {
        Ingredients.Add(product, amount);
    }

    public void Remove(Product product) {
        Ingredients.Remove(product);
    }

    public double GetTotalProtein() {
        double totalProtein = 0;
        foreach (var ingredient in Ingredients) {
            totalProtein += ingredient.Key.Protein * ingredient.Value;
        }
        return totalProtein;
    }

    public double GetTotalFat() {
        double totalFat = 0;
        foreach (var ingredient in Ingredients) {
            totalFat += ingredient.Key.Fat * ingredient.Value;
        }
        return totalFat;
    }

    public double GetTotalCarbohydrates() {
        double totalCarbohydrates = 0;
        foreach (var ingredient in Ingredients) {
            totalCarbohydrates += ingredient.Key.Carbohydrates * ingredient.Value;
        }
        return totalCarbohydrates;
    }
}

public class Recipe {
    public string Name { get; set; }
    public IngredientList Ingredients { get; set; }
    public List<string> Instructions { get; set; }
    public TimeSpan TotalTime { get; set; }
    public int Calories { get; set; }
    public double Protein { get; set; }
    public double Fat { get; set; }
    public double Carbohydrates { get; set; }

    public Recipe() {
        Ingredients = new IngredientList();
        Instructions = new List<string>();
    }

    public override string ToString() {
        var sb = new StringBuilder();
        sb.AppendLine($"- {Name} -");
        sb.AppendLine("Ingredients:");
        foreach (var ingredient in Ingredients.Ingredients) {
            sb.AppendLine($"{ingredient.Key.Name}: {ingredient.Value} {ingredient.Key.Unit.ToString().ToLower()}");
        }
        sb.AppendLine("Instructions:");
        for (int i = 0; i < Instructions.Count; i++) {
            sb.AppendLine($"{i + 1}. {Instructions[i]}");
        }
        sb.AppendLine($"Total Time: {TotalTime}");
        sb.AppendLine($"Calories: {Calories}");
        sb.AppendLine($"Protein: {Protein}");
        sb.AppendLine($"Fat: {Fat}");
        sb.AppendLine($"Carbohydrates: {Carbohydrates}");
        return sb.ToString();
    }
}

public class RecipeBuilder {
    private Recipe _recipe = new Recipe();

    public RecipeBuilder AddName(string name) {
        _recipe.Name = name;
        return this;
    }

    public RecipeBuilder AddIngredient(Product product, double amount) {
        _recipe.Ingredients.Add(product, amount);
        return this;
    }

    public RecipeBuilder RemoveIngredient(Product product) {
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
public interface ICommand
{
    void Execute();
    void Undo();
}

public class AddIngredientCommand : ICommand
{
    private readonly RecipeBuilder recipeBuilder;
    private readonly Product product;
    private readonly double amount;

    public AddIngredientCommand(RecipeBuilder recipeBuilder, Product product, double amount)
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

public class CommandManager
{
    private readonly Stack<ICommand> undoStack = new();
    private readonly Stack<ICommand> redoStack = new();

    public void ExecuteCommand(ICommand command)
    {
        command.Execute();
        undoStack.Push(command);
        redoStack.Clear();
    }

    public void Undo()
    {
        if (undoStack.Count > 0)
        {
            var command = undoStack.Pop();
            command.Undo();
            redoStack.Push(command);
        }
    }

    public void Redo()
    {
        if (redoStack.Count > 0)
        {
            var command = redoStack.Pop();
            command.Execute();
            undoStack.Push(command);
        }
    }
}
}