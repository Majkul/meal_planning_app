using System.Globalization;
using Services;
namespace Program{
public class Program {
    public static void Main()
    {
        //Tworzenie interfejsu
        string opcja="-1";
        DateTime today = DateTime.Today;
        while(opcja!="0"){
        List<DateTime> daysOfWeek = new List<DateTime>();
        for (int i = -3; i <= 3; i++)
        {
            daysOfWeek.Add(today.AddDays(i));
        }

        Console.WriteLine(new string('-', 113));

        for (int i = 0; i < daysOfWeek.Count; i++)
        {
            Console.Write("|");
            string dayName = daysOfWeek[i].ToString("dddd", new CultureInfo("en-US"));

            Console.Write(dayName.PadLeft((15 - dayName.Length) / 2 + dayName.Length)
                                 .PadRight(15));
        }
        Console.WriteLine("|");

        for (int i = 0; i < daysOfWeek.Count; i++)
        {
            Console.Write("|");
            string date = daysOfWeek[i].ToString("dd.MM.yyyy");
            Console.Write(date.PadLeft((15 - date.Length) / 2 + date.Length)
                                .PadRight(15));
        }
        Console.WriteLine("|");

        Console.WriteLine(new string('-', 113));
        string[] meals = {"Breakfast", "Snack I", "Lunch", "Snack II", "Dinner"};

        //Historia posiłków
        string historyFilePath = "mealHistory.json";
        Meal.MealHistory mealHistory = new Meal.MealHistory();
        mealHistory.LoadFromFile(historyFilePath);
        int TotalCalories = 0;
        double TotalProtein = 0;
        double TotalFat = 0;
        double TotalCarbohydrates = 0;
        List<string?> melas_names_in_date = MealServices.GetMealsNamesForDate(mealHistory, today);
        List<Meal.Meal.MealMemento> melas_in_date = MealServices.GetMealsForDate(mealHistory, today);
        for(int i=0; i<5; i++)
        {
            int TotalRecipeCalories = 0;
            double TotalRecipeProtein = 0;
            double TotalRecipeFat = 0;
            double TotalRecipeCarbohydrates = 0;
            Console.Write("|");
            Console.Write(meals[i].PadLeft((111 - meals[i].Length) / 2 + meals[i].Length).PadRight(111));
            Console.WriteLine("|");

            Meal.Meal.MealType mealType = (Meal.Meal.MealType)Array.IndexOf(meals, meals[i]);
            string mealName = melas_names_in_date[i] ?? "No meal";
            if(mealName!="No meal"){
                foreach (var recipe in melas_in_date[i].Recipes) {
                    TotalCalories += recipe.Calories;
                    TotalRecipeCalories += recipe.Calories;

                    TotalRecipeProtein += recipe.Protein;
                    TotalProtein += recipe.Protein;
                    TotalRecipeFat += recipe.Fat;
                    TotalFat += recipe.Fat;
                    TotalRecipeCarbohydrates += recipe.Carbohydrates;
                    TotalCarbohydrates += recipe.Carbohydrates;
                
                }
            }

            Console.Write("|");
            Console.Write(mealName.PadLeft((111 - mealName.Length) / 2 + mealName.Length).PadRight(111));
            Console.WriteLine("|");
            Console.Write("|");
            string summary = $"Kcal: {TotalRecipeCalories}, Prot.: {TotalRecipeProtein}g, Fats: {TotalRecipeFat}g, Carbs: {TotalRecipeCarbohydrates}g";
            Console.Write(summary.PadLeft((111 - summary.Length) / 2 + summary.Length).PadRight(111));
            Console.WriteLine("|");
            Console.WriteLine(new string('-', 113));
        }

        //Kalorie z całego dnia
        Console.Write("|");
        string summaryAllDay = $"Total:   Kcal: {TotalCalories}, Prot.: {TotalProtein}g, Fats: {TotalFat}g, Carbs: {TotalCarbohydrates}g";
        Console.Write(summaryAllDay.PadLeft((111 - summaryAllDay.Length) / 2 + summaryAllDay.Length).PadRight(111));
        Console.WriteLine("|");
        Console.WriteLine(new string('-', 113));
        //Bazy danychy
        var RecipesDatabase = DB.ConnectionManager.getInstance().getConnection<Recipe.Recipe>("Recipes");
        var ProductsDatabase = DB.ConnectionManager.getInstance().getConnection<Product.Product>("Products");

        //Tutaj będzie odczytywanie do bazy danych z plików jakichś przykąłdowych rekodrów
        DataLoader.LoadProductsFromFile("products.json", ProductsDatabase);
        DataLoader.LoadRecipesFromFile("recipes.json", ProductsDatabase, RecipesDatabase);

        //Załaduj produkty i przepisy z historii posiłków
        DataLoader.LoadProductsAndRecipesFromMealsHistory(mealHistory, ProductsDatabase, RecipesDatabase);

        //Menu
        Console.WriteLine("1. Zmień dzień za pomocą < lub >, lub wpisz datę w formacie dd.MM.yyyy");
        Console.WriteLine("2. Dodaj posiłek");
        Console.WriteLine("3. Wyświetlanie listy zakupów");
        Console.WriteLine("4. Wyeksportuj historię posiłków");
        Console.WriteLine("0. Wyjdź");
        opcja = Console.ReadLine()!;
        switch(opcja){
            case "<":
                Console.WriteLine("Zmieniono dzień na poprzedni");
                today = today.AddDays(-1);
                break;
            case ">":
                Console.WriteLine("Zmieniono dzień na następny");
                today = today.AddDays(1);
                break;
            case "2":
                Console.WriteLine("Czy chcesz skopiować posiłek z innego dnia? (tak/nie)");
                string copyMeal = Console.ReadLine()?.ToLower()!;

                if (copyMeal == "tak" || copyMeal == "t") {
                    MealServices.AddMealFromMealHistory(mealHistory, historyFilePath);
                } 
                else if (copyMeal == "nie" || copyMeal == "n") {
                    MealServices.AddNewMeal(mealHistory, historyFilePath, ProductsDatabase, RecipesDatabase);
                }
                else {
                    Console.WriteLine("Nieprawidłowa opcja. Spróbuj ponownie.\n");
                }
                break;

            case "3":
                ShoppingListServices.PickDate(mealHistory, ProductsDatabase);
                break;
            case "4":
                DataExporter.ExportData(mealHistory);
                break;
            case "0":
                break;
            default:
                var previousDate = today;
                if (DateTime.TryParse(opcja, out today)) {
                    Console.WriteLine("Zmieniono dzień na: " + today.ToString("dd.MM.yyyy"));
                } else {
                    today = previousDate;
                    Console.WriteLine("Nieprawidłowa opcja. Spróbuj ponownie.");
                }
                break;
            }
        }
    }

} 
}