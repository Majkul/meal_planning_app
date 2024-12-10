using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using RecipeNamespace;
using DBconnection;
using Meal;
using ShoppingListNamespace;
public class Program {
    public static void Main()
    {
        var recipeBuilder = new RecipeBuilder();
        var commandManager = new CommandManager();

        var egg = new Product("Egg", Product.Units.Units, Product.Categories.Other, 6, 5, 0);
        var milk = new Product("Milk", Product.Units.Milliliters, Product.Categories.Dairy, 3, 2, 4);
        var cheese = new Product("Cheese", Product.Units.Grams, Product.Categories.Dairy, 5, 7, 0);

        commandManager.ExecuteCommand(new AddIngredientCommand(recipeBuilder, egg, 2));
        commandManager.ExecuteCommand(new AddIngredientCommand(recipeBuilder, milk, 50));
        commandManager.ExecuteCommand(new AddIngredientCommand(recipeBuilder, cheese, 50));

        Console.WriteLine("After adding ingredients:");
        Console.WriteLine(recipeBuilder.Build());

        commandManager.Undo();
        Console.WriteLine("\nAfter undoing last action:");
        Console.WriteLine(recipeBuilder.Build());

        commandManager.Redo();
        Console.WriteLine("\nAfter redoing last action:");
        Console.WriteLine(recipeBuilder.Build());
    }
} 