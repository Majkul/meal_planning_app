using System.Text.Json;
namespace Services{
public static class DataLoader{
    public static void LoadProductsAndRecipesFromMealsHistory(Meal.MealHistory mealHistory, DB.DatabaseConnection<Product.Product> ProductsDatabase, DB.DatabaseConnection<Recipe.Recipe> RecipesDatabase) {
        foreach (var meal in mealHistory.History) {
            foreach (var recipe in meal.Recipes) {
                foreach (var ingredient in recipe.Ingredients.Ingredients) {
                    ProductsDatabase.AddRecord(ingredient.Product);
                }
                RecipesDatabase.AddRecord(recipe);
            }
        }
    }
    static public void LoadProductsFromFile(string filePath, DB.DatabaseConnection<Product.Product> ProductsDatabase) {
        List<Product.Product> products;
        if (File.Exists(filePath)) {
            var json = File.ReadAllText(filePath);
            products = JsonSerializer.Deserialize<List<Product.Product>>(json)!;
            if (products == null) {
                Console.WriteLine("Nie udało się odczytać produktów z pliku.");
                products = new List<Product.Product>();
            }
        } else {
            products = new List<Product.Product>();
        }
        foreach (var product in products)
        {
            ProductsDatabase.AddRecord(product);
        }
    }
    static public void LoadRecipesFromFile(string filePath, DB.DatabaseConnection<Product.Product> ProductsDatabase, DB.DatabaseConnection<Recipe.Recipe> RecipesDatabase) {
        List<Recipe.Recipe> recipes;
        if (File.Exists(filePath)) {
            var options = new JsonSerializerOptions{
            Converters = { new Adapters.JsonAdapter.ProductConverter(), new Adapters.JsonAdapter.IngredientListConverter() }
            };
            var json = File.ReadAllText(filePath);
            recipes = JsonSerializer.Deserialize<List<Recipe.Recipe>>(json, options)!;
            if (recipes == null) {
                Console.WriteLine("Nie udało się odczytać przepisów z pliku.");
                recipes = new List<Recipe.Recipe>();
            }
        } else {
            recipes = new List<Recipe.Recipe>();
        }
        foreach (var recipe in recipes)
        {
            foreach (var ingredient in recipe.Ingredients.Ingredients)
            {
                ProductsDatabase.AddRecord(ingredient.Product);
            }
            RecipesDatabase.AddRecord(recipe);
        }
    }
}
}
    