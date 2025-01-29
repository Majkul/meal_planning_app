namespace Services{
public static class IngredientsServices{
    public static void AddIngredient(Recipe.RecipeBuilder recipeBuilder, Recipe.Commands.CommandManager commandManager, DB.DatabaseConnection<Product.Product> ProductsDatabase, DB.DatabaseConnection<Recipe.Recipe> RecipesDatabase) {
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
                int productIndex = VarServices.GetIntFromString("Podaj numer składnika, który chcesz dodać:") - 1;
                if (productIndex >= 0 && productIndex < allProducts.Count)
                {
                    Product.Product selectedProduct = allProducts[productIndex].obj;
                    double selectedAmount = VarServices.GetDoubleFromString("Podaj ilość składnika:");
                    commandManager.ExecuteCommand(new Recipe.Commands.AddIngredientCommand(recipeBuilder, selectedProduct, selectedAmount));
                    Console.WriteLine("Dodano składnik.");
                    return;
                }
                else
                {
                    Console.WriteLine("Nieprawidłowy numer.\n");
                    return;
                }
            }
        } else if (ingredientChoice == "nowy" || ingredientChoice == "n") {
            Console.WriteLine("Podaj nazwę składnika:");
            string productName = Console.ReadLine() ?? $"Product {ProductsDatabase.GetAllRecords().Count + 1}";
            double amount = VarServices.GetDoubleFromString("Podaj ilość składnika:");
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
            double protein = VarServices.GetDoubleFromString("Podaj ilość białka:");
            double fat = VarServices.GetDoubleFromString("Podaj ilość tłuszczu:");
            double carbohydrates = VarServices.GetDoubleFromString("Podaj ilość węglowodanów:");

            Product.Product product = new Product.Product(productName, unit, category, protein, fat, carbohydrates);
        
            Recipe.Commands.ICommand addIngredientCommand = new Recipe.Commands.AddIngredientCommand(recipeBuilder, product, amount);
            commandManager.ExecuteCommand(addIngredientCommand);
            Console.WriteLine("Dodano składnik.");
            return;
        } else {
            Console.WriteLine("Nieprawidłowa opcja. Spróbuj ponownie.\n");
            return;
        }
    }
    public static void DeleteIngredient(Recipe.RecipeBuilder recipeBuilder, Recipe.Commands.CommandManager commandManager) {
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
    }
}
}