using System.Text.Json;
using RecipeNamespace;
namespace Meal{
class Meal {
    public string Name { get; set; }
    public DateTime Date { get; set; }
    public MealType Type { get; set; }
    public List<Recipe> Recipes { get; set; }
    public enum MealType {
        Breakfast,
        SnackI,
        Lunch,
        SnackII,
        Dinner
    }
    public Meal(){
        Recipes = new List<Recipe>();
    }
    public class MealMemento {
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public MealType Type { get; set; }
        public List<Recipe> Recipes { get; set; }
    }

    public MealMemento Save() {
        return new MealMemento {
            Name = Name,
            Date = Date,
            Type = Type,
            Recipes = Recipes
    };
    }

    public void Restore(MealMemento memento) {
        Name = memento.Name;
        Date = memento.Date;
        Type = memento.Type;
        Recipes = memento.Recipes;
    }
}

class MealHistory {
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
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters = { new Product.ProductConverter(), new RecipeNamespace.IngredientListConverter() }
        };

        var json = JsonSerializer.Serialize(History, options);

        File.WriteAllText(filePath, json);
    }


    public void LoadFromFile(string filePath)
{
    if (File.Exists(filePath))
    {
        var options = new JsonSerializerOptions
        {
            Converters = { new Product.ProductConverter(), new RecipeNamespace.IngredientListConverter() }
        };

        var json = File.ReadAllText(filePath);

        History = JsonSerializer.Deserialize<List<Meal.MealMemento>>(json, options);
    }
    else
    {
        History = new List<Meal.MealMemento>();
    }
}
}
}