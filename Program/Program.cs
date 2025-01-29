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

        //Bazy danychy
        var RecipesDatabase = DB.ConnectionManager.getInstance().getConnection<Recipe.Recipe>("Recipes");
        var ProductsDatabase = DB.ConnectionManager.getInstance().getConnection<Product.Product>("Products");

        //Tutaj będzie odczytywanie do bazy danych z plików jakichś przykąłdowych rekodrów
        DataLoader.LoadProductsFromFile("products.json", ProductsDatabase);
        DataLoader.LoadRecipesFromFile("recipes.json", ProductsDatabase, RecipesDatabase);

        //Załaduj produkty i przepisy z historii posiłków
        DataLoader.LoadProductsAndRecipesFromMealsHistory(mealHistory, ProductsDatabase, RecipesDatabase);

        // Obliczanie i wyświetlanie podsumowania
        UIHandler.CalculateAndDisplaySummary(mealHistory, today, meals);

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