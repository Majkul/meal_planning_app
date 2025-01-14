using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Globalization;
using RecipeNamespace;
using DBconnection;
using Meal;
using ShoppingListNamespace;
using Microsoft.VisualBasic;
public class Program {
    public static void AddingRecipes(DatabaseConnection<RecipeNamespace.Recipe> RecipesDatabase, DatabaseConnection<Product> ProductsDatabase){
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
                    Console.WriteLine("Czy chcesz dodać nowy składnik czy wybrać z listy? (nowy/lista)");
                    string ingredientChoice = Console.ReadLine();

                    if (ingredientChoice.ToLower() == "lista")
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
                            Console.WriteLine("Podaj numer składnika, który chcesz dodać:");
                            int productIndex = Convert.ToInt32(Console.ReadLine()) - 1;
                            if (productIndex >= 0 && productIndex < allProducts.Count)
                            {
                                Product selectedProduct = allProducts[productIndex].obj;
                                Console.WriteLine("Podaj ilość składnika:");
                                double selectedAmount = Convert.ToDouble(Console.ReadLine());
                                //ICommand addIngredientCommand = new AddIngredientCommand(recipeBuilder, selectedProduct, selectedAmount);
                                commandManager.ExecuteCommand(new AddIngredientCommand(recipeBuilder, selectedProduct, selectedAmount));
                                Console.WriteLine("Dodano składnik.");
                                break;
                            }
                            else
                            {
                                Console.WriteLine("Nieprawidłowy numer. Dodawanie nowego składnika.");
                                ingredientChoice = "nowy";
                            }
                        }
                    }
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
                    // Console.WriteLine("Podaj nazwę składnika do usunięcia:");
                    // string removeProductName = Console.ReadLine();

                    // Product removeProduct = recipeBuilder.Build().Ingredients.Ingredients
                    //     .Keys.FirstOrDefault(p => p.Name == removeProductName);

                    // if (removeProduct != null)
                    // {
                    //     ICommand removeIngredientCommand = new AddIngredientCommand(recipeBuilder, removeProduct, 0);
                    //     commandManager.ExecuteCommand(removeIngredientCommand);
                    //     Console.WriteLine("Usunięto składnik.");
                    // }
                    // else
                    // {
                    //     Console.WriteLine("Nie znaleziono składnika.");
                    // }
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
    static List<string?> GetMealsNamesForDate(MealHistory mealHistory, DateTime date) {
        // Inicjalizuj listę wyników (5 pozycji, jedna na każdy rodzaj posiłku)
        List<string?> mealsForDate = new List<string?> { null, null, null, null, null };

        // Filtrowanie posiłków z historii dla podanej daty
        var mealsOnDate = GetMealsForDate(mealHistory, date);

        // Przypisywanie nazw posiłków do odpowiednich indeksów w liście wyników
        foreach (var meal in mealsOnDate) {
            mealsForDate[(int)meal.Type] = meal.Name; // Typ posiłku (enum) jako indeks
        }

        return mealsForDate;
    }
    static List<Meal.Meal.MealMemento> GetMealsForDate(MealHistory mealHistory, DateTime date){
        var mealsOnDate = mealHistory.History
            .Where(m => m.Date.Date == date)
            .ToList();
        return mealsOnDate;
    }
    static void LoadProductsAndRecipesFromMealsHistory(MealHistory mealHistory, DatabaseConnection<Product> ProductsDatabase, DatabaseConnection<RecipeNamespace.Recipe> RecipesDatabase) {
        foreach (var meal in mealHistory.History) {
            foreach (var recipe in meal.Recipes) {
                foreach (var ingredient in recipe.Ingredients.Ingredients) {
                    ProductsDatabase.AddRecord(ingredient.Product);
                }
                RecipesDatabase.AddRecord(recipe);
            }
        }
    }
    static public void LoadProductsFromFile(string filePath, DatabaseConnection<Product> ProductsDatabase) {
        List<Product> products;
        if (File.Exists(filePath)) {
            var json = File.ReadAllText(filePath);
            products = JsonSerializer.Deserialize<List<Product>>(json);
        } else {
            products = new List<Product>();
        }
        foreach (var product in products)
        {
            ProductsDatabase.AddRecord(product);
        }
    }
    static public void LoadRecipesFromFile(string filePath, DatabaseConnection<Product> ProductsDatabase, DatabaseConnection<RecipeNamespace.Recipe> RecipesDatabase) {
        List<Recipe> recipes;
        if (File.Exists(filePath)) {
            var options = new JsonSerializerOptions{
            Converters = { new Adapter.ProductConverter(), new Adapter.IngredientListConverter() }
            };
            var json = File.ReadAllText(filePath);
            recipes = JsonSerializer.Deserialize<List<Recipe>>(json, options);
        } else {
            recipes = new List<Recipe>();
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
    static public void OnShopping(ShoppingList shoppingList, DatabaseConnection<Product> ProductsDatabase){
        //CategorizedDisplayDecorator categorizedDisplay = new CategorizedDisplayDecorator(shoppingList);
        MarkAsAddedDecorator markAsAdded = new MarkAsAddedDecorator(shoppingList);
        string option="";
        while(true){
            markAsAdded.DisplayProducts();
            Console.WriteLine("Podaj nazwę produktu, który chcesz dodać do koszyka:");
            Console.WriteLine("0. Wyjdź");
            option = Console.ReadLine();
            if(option=="0"){
                break;
            }
            try{
                Product product = ProductsDatabase.GetRecordByName(option).obj;
                markAsAdded.MarkAsAdded(product);
                Console.WriteLine($"Dodano produkt: {product.Name}");
            }
            catch(System.NullReferenceException){
                Console.WriteLine("Nie znaleziono produktu.");
            }
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
        string[] meals = {"Breakfast", "Snack I", "Lunch", "Snack II", "Dinner"};

        //Historia posiłków
        string historyFilePath = "mealHistory.json";
        MealHistory mealHistory = new MealHistory();
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
            string mealName = melas_names_in_date[i] ?? "Brak posiłku";
            foreach (var recipe in melas_in_date[i].Recipes)
            {
                TotalCalories += recipe.Calories;
                TotalRecipeCalories += recipe.Calories;
                foreach (var ingredient in recipe.Ingredients.Ingredients)
                {
                    TotalRecipeProtein += ingredient.Product.Protein * ingredient.Amount;
                    TotalRecipeFat += ingredient.Product.Fat * ingredient.Amount;
                    TotalRecipeCarbohydrates += ingredient.Product.Carbohydrates * ingredient.Amount;
                    TotalProtein += ingredient.Product.Protein * ingredient.Amount;
                    TotalFat += ingredient.Product.Fat * ingredient.Amount;
                    TotalCarbohydrates += ingredient.Product.Carbohydrates * ingredient.Amount;
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
        var RecipesDatabase = ConnectionManager.getInstance().getConnection<RecipeNamespace.Recipe>("Recipes");
        var ProductsDatabase = ConnectionManager.getInstance().getConnection<Product>("Products");

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

        //TODO obliczanie kalorii i makro z całego dnia i wyświetlenie na dole
        //Menu
        Console.WriteLine("1. Zmień dzień za pomocą < lub >");
        Console.WriteLine("2. Dodaj posiłek");
        Console.WriteLine("3. Wyświetlanie listy zakupów");
        Console.WriteLine("4. Wyeksportuj historię posiłków");
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
                                    Console.WriteLine("Podaj typ posiłku (Breakfast, SnackI, Lunch, SnackII, Dinner):");
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
                        Console.WriteLine("Czy chcesz dodać nowy przepis czy wybrać z listy? (nowy/lista)");
                        string recipeChoice = Console.ReadLine();

                        if (recipeChoice.ToLower() == "lista")
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
                                Console.WriteLine("Podaj numer przepisu, który chcesz dodać:");
                                int recipeIndex = Convert.ToInt32(Console.ReadLine()) - 1;
                                if (recipeIndex >= 0 && recipeIndex < allRecipes.Count)
                                {
                                    Recipe selectedRecipe = allRecipes[recipeIndex].obj;
                                    newMeal.Recipes.Add(selectedRecipe);
                                    Console.WriteLine("Dodano przepis.");
                                    continue;
                                }
                                else
                                {
                                    Console.WriteLine("Nieprawidłowy numer. Dodawanie nowego przepisu.");
                                    recipeChoice = "nowy";
                                }
                            }
                        }
                        AddingRecipes(RecipesDatabase, ProductsDatabase);
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
                Console.WriteLine("Wybierz zakres dat (format: dd/MM/yyyy, np. 22.01.2025):");
                Console.Write("Od: ");
                DateTime startDate = DateTime.Parse(Console.ReadLine());
                Console.Write("Do: ");
                DateTime endDate = DateTime.Parse(Console.ReadLine());
                ShoppingList shoppingList = new ShoppingList();
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
                string addToCart = Console.ReadLine();
                if (addToCart.ToLower() == "tak")
                {
                    OnShopping(shoppingList, ProductsDatabase);
                }
                break;
            case "4":
            Console.WriteLine("Wybierz format eksportu (json/xml/txt):");
            string exportFormat = Console.ReadLine().ToLower();

            var fileManager = new Adapter.FileSaveManager();
            switch (exportFormat)
            {
                case "json":
                    fileManager.SaveToFile("mealHistory.json", mealHistory);
                    break;
                case "txt":
                    fileManager.SaveToFile("mealHistory.txt", mealHistory);
                    Console.WriteLine("Historia posiłków została wyeksportowana do formatu TXT.");
                    break;
                default:
                    Console.WriteLine("Nieprawidłowy format eksportu.");
                    break;
            }
                mealHistory.SaveToFile(".//exportData.json");
                Console.WriteLine("Historia posiłków została wyeksportowana.");
                break;
        }
        }
    }

} 