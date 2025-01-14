using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace RecipeNamespace{
    public class IngredientList
{
    public List<Ingredient> Ingredients { get; set; }

    public IngredientList()
    {
        Ingredients = new List<Ingredient>();
    }

    public void Add(Product product, double amount)
    {
        Ingredients.Add(new Ingredient(product, amount));
    }

    public void Remove(Product product)
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

    public class Ingredient
    {
        public Product Product { get; set; }
        public double Amount { get; set; }

        public Ingredient(Product product, double amount)
        {
            Product = product;
            Amount = amount;
        }
    }

public class IngredientListConverter : JsonConverter<IngredientList>
{
    public override IngredientList Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var ingredientList = new IngredientList();

        using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
        {
            var ingredientsArray = doc.RootElement.EnumerateArray();
            
            foreach (var ingredientElement in ingredientsArray)
            {
                var productJson = ingredientElement.GetProperty("Product");
                var product = JsonSerializer.Deserialize<Product>(productJson.GetRawText(), options);
                var amount = ingredientElement.GetProperty("Amount").GetDouble();
                
                ingredientList.Add(product, amount);
            }
        }

        return ingredientList;
    }

    public override void Write(Utf8JsonWriter writer, IngredientList value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        foreach (var ingredient in value.Ingredients)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("Product");
            JsonSerializer.Serialize(writer, ingredient.Product, options);
            writer.WriteNumber("Amount", ingredient.Amount);
            writer.WriteEndObject();
        }
        writer.WriteEndArray();
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
            sb.AppendLine($"\t{ingredient.Product.Name}: {ingredient.Amount} {ingredient.Product.Unit.ToString().ToLower()}");
        }
        sb.AppendLine("Instructions:");
        for (int i = 0; i < Instructions.Count; i++) {
            sb.AppendLine($"\t{i + 1}. {Instructions[i]}");
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