using System.Text;
using System.Text.Json;
namespace Meal{
public class MealHistory {
    public List<Meal.MealMemento> History { get; set; }

    public MealHistory() {
        History = new List<Meal.MealMemento>();
    }

    public void Add(Meal.MealMemento memento) {
        History.Add(memento);
    }

    public Meal.MealMemento Get(int index) {
        return History[index];
    }

    public void Remove(int index) {
        History.RemoveAt(index);
    }

    public void Clear() {
        History.Clear();
    }

    public void Print() {
        foreach (var memento in History) {
            Console.WriteLine($"Name: {memento.Name}");
        }
    }

    public void SaveToFile(string filePath)
    {
        var fileManager = new Adapters.FileSaveAdapter.FileSaveManager();
        fileManager.SaveToFile(filePath, History);
    }


    public void LoadFromFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            var options = new JsonSerializerOptions
            {
                Converters = { new Adapters.JsonAdapter.ProductConverter(), new Adapters.JsonAdapter.IngredientListConverter() }
            };

            var json = File.ReadAllText(filePath);

            History = JsonSerializer.Deserialize<List<Meal.MealMemento>>(json, options)!;

            if (History == null)
            {
                Console.WriteLine("Error: Could not load data from file.");
                History = new List<Meal.MealMemento>();
            }
        }
        else
        {
            History = new List<Meal.MealMemento>();
        }
    }
        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var memento in History)
            {
                sb.AppendLine($"Name: {memento.Name}");
                sb.AppendLine($"Date: {memento.Date}");
                sb.AppendLine($"Type: {memento.Type}");
                sb.AppendLine("Recipes:");
                foreach (var recipe in memento.Recipes)
                {
                    sb.AppendLine($"  - {recipe.Name}");
                    foreach (var ingredient in recipe.Ingredients)
                    {
                        sb.AppendLine($"    - {ingredient.Product.Name} - {ingredient.Amount} {ingredient.Product.Unit}");
                    }
                }
                sb.AppendLine();
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}