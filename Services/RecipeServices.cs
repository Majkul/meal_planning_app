namespace Services{
public static class RecipeServices{
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
                    IngredientsServices.AddIngredient(recipeBuilder, commandManager, ProductsDatabase, RecipesDatabase);
                    break;
                    
                case "2": // Usuwanie składnika
                    IngredientsServices.DeleteIngredient(recipeBuilder, commandManager);
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
}
}