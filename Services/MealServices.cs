using System.Xml;

namespace Services{
public static class MealServices{
    public static List<Meal.Meal.MealMemento> GetMealsForDate(Meal.MealHistory mealHistory, DateTime date){
        // Inicjalizuj listę wyników (5 pozycji, jedna na każdy rodzaj posiłku)
        List<Meal.Meal.MealMemento> mealsForDate = new List<Meal.Meal.MealMemento> { null!, null!, null!, null!, null! };

        // Filtrowanie posiłków z historii dla podanej daty
        var mealsOnDate = mealHistory.History
            .Where(m => m.Date.Date == date)
            .ToList();

        // Przypisywanie nazw posiłków do odpowiednich indeksów w liście wyników
        foreach (var meal in mealsOnDate) {
            mealsForDate[(int)meal.Type] = meal; // Typ posiłku (enum) jako indeks
        }

        return mealsForDate;
    }
    public static List<string?> GetMealsNamesForDate(Meal.MealHistory mealHistory, DateTime date) {
        // Inicjalizuj listę wyników (5 pozycji, jedna na każdy rodzaj posiłku)
        List<string?> mealsForDate = new List<string?> { null, null, null, null, null };

        // Filtrowanie posiłków z historii dla podanej daty
        var mealsOnDate = mealHistory.History
            .Where(m => m.Date.Date == date)
            .ToList();

        // Przypisywanie nazw posiłków do odpowiednich indeksów w liście wyników
        foreach (var meal in mealsOnDate) {
            mealsForDate[(int)meal.Type] = meal.Name; // Typ posiłku (enum) jako indeks
        }

        return mealsForDate;
    }

    private static void PrintMeals(List<Meal.Meal.MealMemento> meals) {
        Console.WriteLine("Dostępne posiłki:");
        Console.WriteLine(new string('-', 100));
        Console.WriteLine($"| {"Nr",-3} | {"Name",-40} | {"Type",-20} | {"Date",-20} |");
        Console.WriteLine(new string('-', 100));
        for (int i = 0; i < meals.Count; i++) {
            Console.WriteLine($"| {i + 1,-3} | {meals[i].Name,-40} | {meals[i].Type,-20} | {meals[i].Date.ToString("dd.MM.yyyy"),-20} |");
        }
        Console.WriteLine(new string('-', 100));
    }

    public static void AddMealFromMealHistory(Meal.MealHistory mealHistory, string historyFilePath) {
        if (mealHistory.History.Count == 0) {
            Console.WriteLine("Brak zapisanych posiłków w historii.");
            return;
        }

        mealHistory.History = mealHistory.History.OrderBy(m => m.Date).ToList();
        PrintMeals(mealHistory.History);

        int mealIndex = VarServices.GetIntFromString("Podaj numer posiłku, który chcesz skopiować:") - 1;
        if (mealIndex < 0 || mealIndex >= mealHistory.History.Count) {
            Console.WriteLine("Nieprawidłowy numer.");
            return;
        }

        Meal.Meal meal = new Meal.Meal();
        meal.Restore(mealHistory.Get(mealIndex));
        meal.Date = DateTime.Today;

        Console.WriteLine($"Czy chcesz dodać posiłek {meal.Name} do dnia {meal.Date.ToString("dd.MM.yyyy")} i pory {meal.Type}? (tak/nie)");
        string confirmAddMeal = Console.ReadLine()?.ToLower()!;

        if (confirmAddMeal == "tak" || confirmAddMeal == "t")
        {
            mealHistory.Add(meal.Save());
            mealHistory.SaveToFile(historyFilePath);
            Console.WriteLine($"Skopiowano posiłek: {meal.Name}");
        }
        else if (confirmAddMeal == "nie" || confirmAddMeal == "n")
        {
            Console.WriteLine("1.Zmiana pory posiłku");
            Console.WriteLine("2.Anulowanie dodawania posiłku.");
            string confirmAddMeal2 = Console.ReadLine()!;
            if (confirmAddMeal2 == "1")
            {
                meal.Type = VarServices.GetEnumFromString<Meal.Meal.MealType>("Podaj typ posiłku (Breakfast, SnackI, Lunch, SnackII, Dinner):");
                mealHistory.Add(meal.Save());
                mealHistory.SaveToFile(historyFilePath);
                Console.WriteLine($"Skopiowano posiłek: {meal.Name}");
            }
            else
            {
                Console.WriteLine("Anulowano dodawanie posiłku.");
            }
        }
        else
        {
            Console.WriteLine("Anulowano dodawanie posiłku.");
        }
    }
    public static void AddNewMeal(Meal.MealHistory mealHistory, string historyFilePath, DB.DatabaseConnection<Product.Product> ProductsDatabase, DB.DatabaseConnection<Recipe.Recipe> RecipesDatabase) {
        Meal.Meal newMeal = new Meal.Meal();
        Console.WriteLine("Podaj nazwę posiłku:");
        newMeal.Name = Console.ReadLine() ?? $"Meal {mealHistory.History.Count + 1}";
        
        int recipeCount = VarServices.GetIntFromString("Ile przepisów chcesz dodać do posiłku?");
        for (int i = 0; i < recipeCount; i++) {
            Console.WriteLine($"Dodawanie przepisu {i + 1} z {recipeCount}:");
            Console.WriteLine("Czy chcesz dodać nowy przepis czy wybrać z listy? (nowy/lista)");
            string recipeChoice = Console.ReadLine()?.ToLower()!;

            if (recipeChoice == "lista" || recipeChoice == "l")
            {
                AddMealFromList(newMeal, RecipesDatabase);
            }
            else if (recipeChoice == "nowy" || recipeChoice == "n")
            {
                RecipeServices.AddingRecipes(RecipesDatabase, ProductsDatabase);
                var newestRecipe = RecipesDatabase.GetLastRecord();
                if (newestRecipe != null)
                {
                    newMeal.Recipes.Add(newestRecipe);
                } else {
                    Console.WriteLine("Nie udało się dodać przepisu.");
                    i--;
                    continue;
                }
            }
            else
            {
                Console.WriteLine("Nieprawidłowa opcja. Spróbuj ponownie.\n");
                i--;
            }
        }

        newMeal.Type = VarServices.GetEnumFromString<Meal.Meal.MealType>("Podaj typ posiłku (Breakfast, SnackI, Lunch, SnackII, Dinner):");
        newMeal.Date = DateTime.Today;

        Console.WriteLine($"Czy chcesz dodać posiłek {newMeal.Name} do dnia {newMeal.Date.ToString("dd.MM.yyyy")} i pory {newMeal.Type}? (tak/nie)");
        string confirmAddMeal = Console.ReadLine()?.ToLower()!;
        if (confirmAddMeal == "tak" || confirmAddMeal == "t")
        {
            mealHistory.Add(newMeal.Save());
            mealHistory.SaveToFile(historyFilePath);
            Console.WriteLine($"Dodano nowy posiłek: {newMeal.Name}");
        }
        else
        {
            Console.WriteLine("Anulowano dodawanie posiłku.");
        }
        
    }    

    private static void AddMealFromList(Meal.Meal newMeal, DB.DatabaseConnection<Recipe.Recipe> RecipesDatabase) {
        var allRecipes = RecipesDatabase.GetAllRecords();
        if (allRecipes.Count == 0)
        {
            Console.WriteLine("Brak dostępnych przepisów w bazie danych.");
        }
        else
        {
            Console.WriteLine("Dostępne przepisy:");
            for (int j = 0; j < allRecipes.Count; j++)
            {
                Console.WriteLine($"{j + 1}. {allRecipes[j].obj.Name}");
            }
            int recipeIndex = VarServices.GetIntFromString("Podaj numer przepisu, który chcesz dodać:") - 1;
            if (recipeIndex >= 0 && recipeIndex < allRecipes.Count)
            {
                Recipe.Recipe selectedRecipe = allRecipes[recipeIndex].obj;
                newMeal.Recipes.Add(selectedRecipe);
                Console.WriteLine("Dodano przepis.");
            }
            else
            {
                Console.WriteLine("Nieprawidłowy numer.\n");
            }
        }
    }
}
}