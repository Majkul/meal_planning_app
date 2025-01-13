using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using RecipeNamespace;
using DBconnection;
using Meal;
using ShoppingListNamespace;
using Microsoft.VisualBasic;
public class Program {
    public static void AddingRecipes(DatabaseConnection<RecipeNamespace.Recipe> RecipesDatabase){
        Console.WriteLine("Dodawanie nowego przepisu...");
        RecipeBuilder recipeBuilder = new RecipeBuilder();
        CommandManager commandManager = new CommandManager();


        Console.WriteLine("Podaj nazwę przepisu:");
        string recipeName = Console.ReadLine();
        recipeBuilder.AddName(recipeName); //Dodac sprawdzanie czy przepis istnieje

        bool addingIngredients = true;
        while (addingIngredients){
            Console.WriteLine("Co chcesz zrobić?");
            Console.WriteLine("1. Dodaj składnik");
            Console.WriteLine("2. Usuń składnik");
            Console.WriteLine("3. Dodaj krok instrukcji");
            Console.WriteLine("4. Cofnij ostatnią operację");
            Console.WriteLine("5. Ponów ostatnią operację");
            Console.WriteLine("6. Zakończ i zapisz przepis");
            string subChoice = Console.ReadLine();

            switch (subChoice)
            {
                case "1": // Dodawanie składnika
                    Console.WriteLine("Podaj nazwę składnika:");
                    string productName = Console.ReadLine();
                    Console.WriteLine("Podaj ilość składnika:");
                    double amount = Convert.ToDouble(Console.ReadLine());
                    Product.Units unit;
                    while (true)
                    {
                        Console.WriteLine("Podaj jednostkę składnika:");
                        string unitInput = Console.ReadLine();
                        if (Enum.TryParse(unitInput, true, out unit))
                        {
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Nieprawidłowa jednostka. Spróbuj ponownie.");
                        }
                    }
                    Product.Categories category;
                    while (true)
                    {
                        Console.WriteLine("Podaj kategorię składnika:");
                        string categoryInput = Console.ReadLine();
                        if (Enum.TryParse(categoryInput, true, out category))
                        {
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Nieprawidłowa jednostka. Spróbuj ponownie.");
                        }
                    }
                    Console.WriteLine("Podaj ilość białka:");
                    double protein = Convert.ToDouble(Console.ReadLine());
                    Console.WriteLine("Podaj ilość tłuszczu:");
                    double fat = Convert.ToDouble(Console.ReadLine());
                    Console.WriteLine("Podaj ilość węglowodanów:");
                    double carbohydrates = Convert.ToDouble(Console.ReadLine());

                    Product product = new Product(productName, unit, category, protein, fat, carbohydrates);
                   
                    ICommand addIngredientCommand = new AddIngredientCommand(recipeBuilder, product, amount);
                    commandManager.ExecuteCommand(addIngredientCommand);
                    Console.WriteLine("Dodano składnik.");
                    break;

                case "2": // Usuwanie składnika
                    Console.WriteLine("Podaj nazwę składnika do usunięcia:");
                    string removeProductName = Console.ReadLine();

                    Product removeProduct = recipeBuilder.Build().Ingredients.Ingredients
                        .Keys.FirstOrDefault(p => p.Name == removeProductName);

                    if (removeProduct != null)
                    {
                        ICommand removeIngredientCommand = new AddIngredientCommand(recipeBuilder, removeProduct, 0);
                        commandManager.ExecuteCommand(removeIngredientCommand);
                        Console.WriteLine("Usunięto składnik.");
                    }
                    else
                    {
                        Console.WriteLine("Nie znaleziono składnika.");
                    }
                    break;

                case "3": // Dodawanie kroku instrukcji
                    Console.WriteLine("Podaj instrukcję:");
                    string instruction = Console.ReadLine();
                    recipeBuilder.AddInstructionStep(instruction);
                    Console.WriteLine("Dodano krok instrukcji.");
                    break;

                case "4": // Cofnięcie ostatniej operacji
                    commandManager.Undo();
                    Console.WriteLine("Cofnięto ostatnią operację.");
                    break;

                case "5": // Ponowienie ostatniej operacji
                    commandManager.Redo();
                    Console.WriteLine("Ponowiono ostatnią operację.");
                    break;

                case "6": // Zakończenie dodawania przepisu
                    recipeBuilder.CalculateNutrition();
                    Recipe newRecipe = recipeBuilder.Build();
                    Console.WriteLine("Zakończono dodawanie przepisu:");
                    Console.WriteLine(newRecipe.ToString());
                    RecipesDatabase.AddRecord(newRecipe); // Zapisz przepis do bazy danych
                    addingIngredients = false;
                    break;

                default:
                    Console.WriteLine("Nieprawidłowa opcja. Spróbuj ponownie.");
                    break;
            }
        }
    }
    static List<string?> GetMealsForDate(MealHistory mealHistory, DateTime date) {
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
            string dayName = daysOfWeek[i].ToString("dddd");
            if (daysOfWeek[i] == today)
            {
                dayName = dayName;
            }

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
        string[] meals = {"Śniadanie", "Drugie śniadanie", "Obiad", "Podwieczorek", "Kolacja"};

        //Historia posiłków
        string historyFilePath = "mealHistory.json";
        MealHistory mealHistory = new MealHistory();
        mealHistory.LoadFromFile(historyFilePath);
        List<string?> melas_in_date = GetMealsForDate(mealHistory, today);
        for(int i=0; i<5; i++)
        {
            Console.Write("|");
            Console.Write(meals[i].PadLeft((111 - meals[i].Length) / 2 + meals[i].Length).PadRight(111));
            Console.WriteLine("|");

            Meal.Meal.MealType mealType = (Meal.Meal.MealType)Array.IndexOf(meals, meals[i]);
            string mealName = melas_in_date[i] ?? "Brak posiłku";

            Console.Write("|");
            Console.Write(mealName.PadLeft((111 - mealName.Length) / 2 + mealName.Length).PadRight(111));
            Console.WriteLine("|");
            Console.WriteLine(new string('-', 113));
        }
        //Bazy danychy
        var RecipesDatabase = ConnectionManager.getInstance().getConnection<RecipeNamespace.Recipe>("Recipes");

        //Tutaj będzie odczytywanie do bazy danych z plików jakichś przykąłdowych rekodrów



        // Załaduj historię z pliku, jeśli istnieje
        if (File.Exists(historyFilePath)) {
            mealHistory.LoadFromFile(historyFilePath);
            Console.WriteLine("Historia posiłków załadowana.");
        } else {
            Console.WriteLine("Brak zapisanej historii. Utworzono nową.");
        }
        //TODO obliczanie kalorii i makro z całego dnia i wyświetlenie na dole
        //Menu
        Console.WriteLine("1. Zmień dzień za pomocą < lub >");
        Console.WriteLine("2. Dodaj posiłek");
        Console.WriteLine("0. Wyjdź");
        opcja = Console.ReadLine();
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
                string copyMeal = Console.ReadLine();

                if (copyMeal.ToLower() == "tak") { //TODO niech wyświetlają się unikatowe posiłki, w sensie żeby się nie powtarzały przez to że są te same posiłki na różne daty
                    mealHistory.LoadFromFile("mealHistory.json");
                    // Wyświetl historię posiłków
                    if (mealHistory.History.Count == 0) {
                        Console.WriteLine("Brak zapisanych posiłków w historii.");
                    } else {
                        Console.WriteLine("Dostępne posiłki:");
                        for (int i = 0; i < mealHistory.History.Count; i++) {
                            Console.WriteLine($"{i + 1}: {mealHistory.History[i].Name}");
                        }

                        Console.WriteLine("Podaj numer posiłku, który chcesz skopiować:");
                        int mealIndex = Convert.ToInt32(Console.ReadLine()) - 1;
                        if (mealIndex >= 0 && mealIndex < mealHistory.History.Count) {
                            Meal.Meal meal = new Meal.Meal();
                            meal.Restore(mealHistory.Get(mealIndex));
                            meal.Date = today;
                            Console.WriteLine($"Czy chcesz dodać posiłek {meal.Name} do dnia {meal.Date.ToString("dd.MM.yyyy")} i pory {meal.Type}? (tak/nie)");
                            string confirmAddMeal = Console.ReadLine();
                            if (confirmAddMeal.ToLower() == "tak")
                            {
                                mealHistory.Add(meal.Save());
                                mealHistory.SaveToFile(historyFilePath);
                                Console.WriteLine($"Skopiowano posiłek: {meal.Name}");
                            }
                            else
                            {
                                Console.WriteLine("1.Zmiana pory posiłku");
                                Console.WriteLine("2.Anulowanie dodawania posiłku.");
                                string confirmAddMeal2 = Console.ReadLine();
                                if (confirmAddMeal2 == "1")
                                {
                                    Console.WriteLine("Podaj typ posiłku (Śniadanie, Drugie śniadanie, Obiad, Podwieczorek, Kolacja):");
                                    Meal.Meal.MealType mealType;
                                    while (true)
                                    {
                                        string mealTypeInput = Console.ReadLine();
                                        if (Enum.TryParse(mealTypeInput, true, out mealType))
                                        {
                                            meal.Type = mealType;
                                            break;
                                        }
                                        else
                                        {
                                            Console.WriteLine("Nieprawidłowy typ posiłku. Spróbuj ponownie.");
                                        }
                                    }
                                    mealHistory.Add(meal.Save());
                                    mealHistory.SaveToFile(historyFilePath);
                                    Console.WriteLine($"Skopiowano posiłek: {meal.Name}");
                                }
                                else
                                {
                                    Console.WriteLine("Anulowano dodawanie posiłku.");
                                }
                            }
                            Console.WriteLine($"Skopiowano posiłek: {meal.Name}");

                        } else {
                            Console.WriteLine("Nieprawidłowy numer.");
                        }
                    }
                } 
                else {
                    Meal.Meal newMeal = new Meal.Meal();
                    Console.WriteLine("Podaj nazwę posiłku:");
                    newMeal.Name = Console.ReadLine();
                    Console.WriteLine("Ile przepisów chcesz dodać do posiłku?");
                    int recipeCount = Convert.ToInt32(Console.ReadLine());

                    for (int i = 0; i < recipeCount; i++) {
                        Console.WriteLine($"Dodawanie przepisu {i + 1} z {recipeCount}:");
                        AddingRecipes(RecipesDatabase);
                        var test = RecipesDatabase.GetLastRecord();
                        newMeal.Recipes.Add(test);
                    }
                    Console.WriteLine("Podaj typ posiłku (Śniadanie, Drugie śniadanie, Obiad, Podwieczorek, Kolacja):");
                    Meal.Meal.MealType mealType;
                    while (true)
                    {
                        string mealTypeInput = Console.ReadLine();
                        if (Enum.TryParse(mealTypeInput, true, out mealType))
                        {
                            newMeal.Type = mealType;
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Nieprawidłowy typ posiłku. Spróbuj ponownie.");
                        }
                    }
                    newMeal.Date = today;
                    Console.WriteLine($"Czy chcesz dodać posiłek {newMeal.Name} do dnia {newMeal.Date.ToString("dd.MM.yyyy")} i pory {newMeal.Type}? (tak/nie)");
                    string confirmAddMeal = Console.ReadLine();
                    if (confirmAddMeal.ToLower() == "tak")
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
                break;

            case "3":
                break;
        }
        }
    }

} 