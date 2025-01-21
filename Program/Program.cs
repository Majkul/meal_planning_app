using System.Text.Json;
using System.Globalization;
using ShoppingList.Decorators;
namespace Program{
public class Program {
    private static double GetDoubleFromString(string message) {
        while (true) {
            Console.WriteLine(message);
            try {
                return Convert.ToDouble(Console.ReadLine());
            } catch (FormatException) {
                Console.WriteLine("Nieprawidłowa liczba. Spróbuj ponownie.\n");
            }
        }
    }

    private static int GetIntFromString(string message) {
        while (true) {
            Console.WriteLine(message);
            try {
                return Convert.ToInt32(Console.ReadLine());
            } catch (FormatException) {
                Console.WriteLine("Nieprawidłowa liczba. Spróbuj ponownie.\n");
            }
        }
    }

    private static DateTime GetDateFromString(string message) {
        while (true) {
            Console.WriteLine(message);
            try {
                return Convert.ToDateTime(Console.ReadLine());
            } catch (FormatException) {
                Console.WriteLine("Nieprawidłowa data. Spróbuj ponownie.\n");
            }
        }
    }

    public static void AddingRecipes(DB.DatabaseConnection<Recipe.Recipe> RecipesDatabase, DB.DatabaseConnection<Product.Product> ProductsDatabase){
        Console.WriteLine("Dodawanie nowego przepisu...");
        Recipe.RecipeBuilder recipeBuilder = new Recipe.RecipeBuilder();
        Recipe.Commands.CommandManager commandManager = new Recipe.Commands.CommandManager();


        Console.WriteLine("Podaj nazwę przepisu:");
        string recipeName;
        while(true) {
            recipeName = Console.ReadLine() ?? $"Recipe {RecipesDatabase.GetAllRecords().Count + 1}";
            var existingRecipe = RecipesDatabase.GetAllRecords().FirstOrDefault(r => r.obj.Name == recipeName);
            if (existingRecipe == null)
            {
                break;
            }
            Console.WriteLine("Przepis o tej nazwie już istnieje. Podaj inną nazwę:");
        }

        recipeBuilder.AddName(recipeName);

        bool addingIngredients = true;
        
        while (addingIngredients){
            Console.WriteLine("Aktualne składniki w przepisie:");
            foreach (var ingredient in recipeBuilder.Build().Ingredients.Ingredients)
            {
                Console.WriteLine($"{ingredient.Product.Name} - {ingredient.Amount} {ingredient.Product.Unit}");
            }
            Console.WriteLine("Co chcesz zrobić?");
            Console.WriteLine("1. Dodaj składnik");
            Console.WriteLine("2. Usuń składnik");
            Console.WriteLine("3. Dodaj krok instrukcji");
            Console.WriteLine("4. Cofnij ostatnią operację");
            Console.WriteLine("5. Ponów ostatnią operację");
            Console.WriteLine("6. Zakończ i zapisz przepis");
            string subChoice = Console.ReadLine()!;

            switch (subChoice)
            {
                case "1": // Dodawanie składnika
                    Console.WriteLine("Czy chcesz dodać nowy składnik czy wybrać z listy? (nowy/lista)");
                    string ingredientChoice = Console.ReadLine()?.ToLower()!;

                    if (ingredientChoice == "lista" || ingredientChoice == "l")
                    {
                        var allProducts = ProductsDatabase.GetAllRecords();
                        if (allProducts.Count == 0)
                        {
                            Console.WriteLine("Brak dostępnych składników w bazie danych.");
                            ingredientChoice = "nowy";
                        }
                        else
                        {
                            Console.WriteLine("Dostępne składniki:");
                            for (int i = 0; i < allProducts.Count; i++)
                            {
                                Console.WriteLine($"{i + 1}. {allProducts[i].obj.Name}");
                            }
                            int productIndex = GetIntFromString("Podaj numer składnika, który chcesz dodać:") - 1;
                            if (productIndex >= 0 && productIndex < allProducts.Count)
                            {
                                Product.Product selectedProduct = allProducts[productIndex].obj;
                                double selectedAmount = GetDoubleFromString("Podaj ilość składnika:");
                                commandManager.ExecuteCommand(new Recipe.Commands.AddIngredientCommand(recipeBuilder, selectedProduct, selectedAmount));
                                Console.WriteLine("Dodano składnik.");
                                break;
                            }
                            else
                            {
                                Console.WriteLine("Nieprawidłowy numer.\n");
                                break;
                            }
                        }
                    } else if (ingredientChoice == "nowy" || ingredientChoice == "n") {
                        Console.WriteLine("Podaj nazwę składnika:");
                        string productName = Console.ReadLine() ?? $"Product {ProductsDatabase.GetAllRecords().Count + 1}";
                        double amount = GetDoubleFromString("Podaj ilość składnika:");
                        Product.Product.Units unit;
                        while (true)
                        {
                            Console.WriteLine("Podaj jednostkę składnika: (Grams, Milliliters, Units)");
                            string unitInput = Console.ReadLine()!;
                            if (Enum.TryParse(unitInput, true, out unit))
                            {
                                break;
                            }
                            else
                            {
                                Console.WriteLine("Nieprawidłowa jednostka. Spróbuj ponownie.\n");
                            }
                        }
                        Product.Product.Categories category;
                        while (true)
                        {
                            Console.WriteLine("Podaj kategorię składnika: (Fruit, Vegetable, Meat, Dairy, Grain, Other)");
                            string categoryInput = Console.ReadLine()!;
                            if (Enum.TryParse(categoryInput, true, out category))
                            {
                                break;
                            }
                            else
                            {
                                Console.WriteLine("Nieprawidłowa jednostka. Spróbuj ponownie.\n");
                            }
                        }
                        double protein = GetDoubleFromString("Podaj ilość białka:");
                        double fat = GetDoubleFromString("Podaj ilość tłuszczu:");
                        double carbohydrates = GetDoubleFromString("Podaj ilość węglowodanów:");

                        Product.Product product = new Product.Product(productName, unit, category, protein, fat, carbohydrates);
                    
                        Recipe.Commands.ICommand addIngredientCommand = new Recipe.Commands.AddIngredientCommand(recipeBuilder, product, amount);
                        commandManager.ExecuteCommand(addIngredientCommand);
                        Console.WriteLine("Dodano składnik.");
                        break;
                    } else {
                        Console.WriteLine("Nieprawidłowa opcja. Spróbuj ponownie.\n");
                        break;
                    }
                    break;
                    
                case "2": // Usuwanie składnika
                    Console.WriteLine("Podaj nazwę składnika do usunięcia:");
                    string ingredientName = Console.ReadLine()?.ToLower()!;
                    Product.Product ingredientToRemove = recipeBuilder.Build().Ingredients.Ingredients.FirstOrDefault(i => i.Product.Name.ToLower() == ingredientName)?.Product!;

                    if (ingredientToRemove != null)
                    {
                        Recipe.Commands.ICommand deleteIngredientCommand = new Recipe.Commands.DeleteIngredientCommand(recipeBuilder, ingredientToRemove);
                        commandManager.ExecuteCommand(deleteIngredientCommand);
                        Console.WriteLine("Usunięto składnik.");
                    }
                    else
                    {
                        Console.WriteLine("Nie znaleziono składnika.");
                    }
                    break;

                case "3": // Dodawanie kroku instrukcji
                    Console.WriteLine("Podaj instrukcję:");
                    string instruction = Console.ReadLine() ?? $"Instruction {recipeBuilder.Build().Instructions.Count + 1}";
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
                    Recipe.Recipe newRecipe = recipeBuilder.Build();
                    Console.WriteLine("Zakończono dodawanie przepisu:");
                    Console.WriteLine(newRecipe.ToString());
                    RecipesDatabase.AddRecord(newRecipe);
                    addingIngredients = false;
                    break;

                default:
                    Console.WriteLine("Nieprawidłowa opcja. Spróbuj ponownie.\n");
                    break;
            }
        }
    }
    static List<string?> GetMealsNamesForDate(Meal.MealHistory mealHistory, DateTime date) {
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
    static List<Meal.Meal.MealMemento> GetMealsForDate(Meal.MealHistory mealHistory, DateTime date){
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
    static void LoadProductsAndRecipesFromMealsHistory(Meal.MealHistory mealHistory, DB.DatabaseConnection<Product.Product> ProductsDatabase, DB.DatabaseConnection<Recipe.Recipe> RecipesDatabase) {
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
    static public void OnShopping(ShoppingList.ShoppingList shoppingList, DB.DatabaseConnection<Product.Product> ProductsDatabase){
        MarkAsAddedDecorator markAsAdded = new MarkAsAddedDecorator(shoppingList);
        string option="";
        while(true){
            markAsAdded.DisplayProducts();
            Console.WriteLine("Podaj nazwę produktu, który chcesz dodać do koszyka:");
            Console.WriteLine("0. Wyjdź");
            option = Console.ReadLine()!;
            if(option=="0"){
                break;
            }
            Product.Product product = ProductsDatabase.GetRecordByName(option)?.obj!;
            if (product == null) {
                Console.WriteLine("Nie znaleziono produktu o podanej nazwie.");
                continue;
            }
            markAsAdded.MarkAsAdded(product);
            Console.WriteLine($"Dodano produkt: {product.Name}");
        }
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
        List<string?> melas_names_in_date = GetMealsNamesForDate(mealHistory, today);
        List<Meal.Meal.MealMemento> melas_in_date = GetMealsForDate(mealHistory, today);
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
        LoadProductsFromFile("products.json", ProductsDatabase);
        LoadRecipesFromFile("recipes.json", ProductsDatabase, RecipesDatabase);
        // ProductsDatabase.ShowAllRecords();
        // RecipesDatabase.ShowAllRecords();

        // Załaduj historię z pliku, jeśli istnieje
        if (File.Exists(historyFilePath)) {
            mealHistory.LoadFromFile(historyFilePath);
            Console.WriteLine("Historia posiłków załadowana.");
        } else {
            Console.WriteLine("Brak zapisanej historii. Utworzono nową.");
        }
        //Załaduj produkty i przepisy z historii posiłków
        LoadProductsAndRecipesFromMealsHistory(mealHistory, ProductsDatabase, RecipesDatabase);

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

                if (copyMeal == "tak" || copyMeal == "t") { //TODO niech wyświetlają się unikatowe posiłki, w sensie żeby się nie powtarzały przez to że są te same posiłki na różne daty

                    // Wyświetl historię posiłków
                    if (mealHistory.History.Count == 0) {
                        Console.WriteLine("Brak zapisanych posiłków w historii.");
                    } else {
                        mealHistory.History = mealHistory.History.OrderBy(m => m.Date).ToList();
                        Console.WriteLine("Dostępne posiłki:");
                        Console.WriteLine(new string('-', 100));
                        Console.WriteLine($"| {"Nr",-3} | {"Name",-40} | {"Type",-20} | {"Date",-20} |");
                        Console.WriteLine(new string('-', 100));
                        for (int i = 0; i < mealHistory.History.Count; i++) {
                            Console.WriteLine($"| {i + 1,-3} | {mealHistory.History[i].Name,-40} | {mealHistory.History[i].Type,-20} | {mealHistory.History[i].Date.ToString("dd.MM.yyyy"),-20} |");
                        }
                        Console.WriteLine(new string('-', 100));

                        int mealIndex = GetIntFromString("Podaj numer posiłku, który chcesz skopiować:") - 1;
                        if (mealIndex >= 0 && mealIndex < mealHistory.History.Count) {
                            Meal.Meal meal = new Meal.Meal();
                            meal.Restore(mealHistory.Get(mealIndex));
                            meal.Date = today;
                            Console.WriteLine($"Czy chcesz dodać posiłek {meal.Name} do dnia {meal.Date.ToString("dd.MM.yyyy")} i pory {meal.Type}? (tak/nie)");
                            string confirmAddMeal = Console.ReadLine()?.ToLower()!;
                            if (confirmAddMeal == "tak" || confirmAddMeal == "t")
                            {
                                mealHistory.Add(meal.Save());
                                mealHistory.SaveToFile(historyFilePath);
                                Console.WriteLine($"Skopiowano posiłek: {meal.Name}");
                            }
                            else
                            {
                                Console.WriteLine("1.Zmiana pory posiłku");
                                Console.WriteLine("2.Anulowanie dodawania posiłku.");
                                string confirmAddMeal2 = Console.ReadLine()!;
                                if (confirmAddMeal2 == "1")
                                {
                                    Console.WriteLine("Podaj typ posiłku (Breakfast, SnackI, Lunch, SnackII, Dinner):");
                                    Meal.Meal.MealType mealType;
                                    while (true)
                                    {
                                        string mealTypeInput = Console.ReadLine()!;
                                        if (Enum.TryParse(mealTypeInput, true, out mealType))
                                        {
                                            meal.Type = mealType;
                                            break;
                                        }
                                        else
                                        {
                                            Console.WriteLine("Nieprawidłowy typ posiłku. Spróbuj ponownie.\n");
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

                        } else {
                            Console.WriteLine("Nieprawidłowy numer.\n");
                        }
                    }
                } 
                else if (copyMeal == "nie" || copyMeal == "n") {
                    Meal.Meal newMeal = new Meal.Meal();
                    Console.WriteLine("Podaj nazwę posiłku:");
                    newMeal.Name = Console.ReadLine() ?? $"Meal {mealHistory.History.Count + 1}";
                    int recipeCount = GetIntFromString("Ile przepisów chcesz dodać do posiłku?");

                    for (int i = 0; i < recipeCount; i++) {
                        Console.WriteLine($"Dodawanie przepisu {i + 1} z {recipeCount}:");
                        Console.WriteLine("Czy chcesz dodać nowy przepis czy wybrać z listy? (nowy/lista)");
                        string recipeChoice = Console.ReadLine()?.ToLower()!;

                        if (recipeChoice == "lista" || recipeChoice == "l")
                        {
                            var allRecipes = RecipesDatabase.GetAllRecords();
                            if (allRecipes.Count == 0)
                            {
                                Console.WriteLine("Brak dostępnych przepisów w bazie danych.");
                                recipeChoice = "nowy";
                            }
                            else
                            {
                                Console.WriteLine("Dostępne przepisy:");
                                for (int j = 0; j < allRecipes.Count; j++)
                                {
                                    Console.WriteLine($"{j + 1}. {allRecipes[j].obj.Name}");
                                }
                                int recipeIndex = GetIntFromString("Podaj numer przepisu, który chcesz dodać:") - 1;
                                if (recipeIndex >= 0 && recipeIndex < allRecipes.Count)
                                {
                                    Recipe.Recipe selectedRecipe = allRecipes[recipeIndex].obj;
                                    newMeal.Recipes.Add(selectedRecipe);
                                    Console.WriteLine("Dodano przepis.");
                                    continue;
                                }
                                else
                                {
                                    Console.WriteLine("Nieprawidłowy numer. Dodawanie nowego przepisu.\n");
                                    recipeChoice = "nowy";
                                }
                            }
                        }
                        else if (recipeChoice == "nowy" || recipeChoice == "n")
                        {
                            AddingRecipes(RecipesDatabase, ProductsDatabase);
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
                    Console.WriteLine("Podaj typ posiłku (Śniadanie, Drugie śniadanie, Obiad, Podwieczorek, Kolacja):");
                    Meal.Meal.MealType mealType;
                    while (true)
                    {
                        string mealTypeInput = Console.ReadLine()!;
                        if (Enum.TryParse(mealTypeInput, true, out mealType))
                        {
                            newMeal.Type = mealType;
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Nieprawidłowy typ posiłku. Spróbuj ponownie.\n");
                        }
                    }
                    newMeal.Date = today;
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
                else {
                    Console.WriteLine("Nieprawidłowa opcja. Spróbuj ponownie.\n");
                }
                break;

            case "3":
                Console.WriteLine("Wybierz zakres dat (format: dd/MM/yyyy, np. 22.01.2025):");
                DateTime startDate = GetDateFromString("Od: ");
                DateTime endDate = GetDateFromString("Do: ");
                ShoppingList.ShoppingList shoppingList = new ShoppingList.ShoppingList();
                var mealsInRange = mealHistory.History
                    .Where(m => m.Date.Date >= startDate.Date && m.Date.Date <= endDate.Date)
                    .ToList();

                foreach (var meal in mealsInRange)
                {
                    foreach(var recip in meal.Recipes)
                    {
                        shoppingList.GenerateFromRecipe(recip);
                    }
                }
                CategorizedDisplayDecorator categorizedDisplay = new CategorizedDisplayDecorator(shoppingList);
                categorizedDisplay.DisplayProducts();
                Console.WriteLine("Czy wyświetlić w trybie dodawania do koszyka? (tak/nie)");
                string addToCart = Console.ReadLine()?.ToLower()!;
                if (addToCart == "tak" || addToCart == "t")
                {
                    OnShopping(shoppingList, ProductsDatabase);
                }
                else
                {
                    Console.WriteLine("Anulowano dodawanie do koszyka.");
                }
                break;
            case "4":
                Console.WriteLine("Wybierz format eksportu (json/xml/txt):");
                string exportFormat = Console.ReadLine()?.ToLower()!;

                var fileManager = new Adapters.FileSaveAdapter.FileSaveManager();
                switch (exportFormat)
                {
                    case "json":
                        fileManager.SaveToFile($"export-{today:dd.MM.yyyy-HH.mm.ss}.json", mealHistory);
                        Console.WriteLine("Historia posiłków została wyeksportowana do formatu JSON.");
                        break;
                    case "txt":
                        fileManager.SaveToFile($"export-{today:dd.MM.yyyy-HH.mm.ss}.txt", mealHistory);
                        Console.WriteLine("Historia posiłków została wyeksportowana do formatu TXT.");
                        break;
                    default:
                        Console.WriteLine("Nieprawidłowy format eksportu.\n");
                        break;
                }
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