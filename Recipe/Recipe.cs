using System.Text;
namespace Recipe{
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
}