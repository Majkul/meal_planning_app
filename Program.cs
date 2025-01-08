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
        string[] meals = { "Śniadanie", "Drugie śniadanie", "Obiad", "Podwieczorek", "Kolacja" };

        foreach (string meal in meals)
        {
            Console.Write("|");
            Console.Write(meal.PadLeft((111 - meal.Length) / 2 + meal.Length).PadRight(111));
            Console.WriteLine("|");
            Console.WriteLine(new string('-', 113));

            for (int i = 0; i < 1; i++)
            {
                Console.Write("|");
                Console.Write(new string(' ', 111));
                Console.WriteLine("|");
            }
            Console.WriteLine(new string('-', 113));
        }
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
                // Console.WriteLine("Podaj nazwę posiłku");
                // string mealName = Console.ReadLine();
                // Meal.Meal meal = new Meal.Meal();
                // meal.Name = mealName;
                // Console.WriteLine("Podaj ilość przepisów");
                // int n = Convert.ToInt32(Console.ReadLine());
                // for (int i = 0; i < n; i++)
                // {
                //     Console.WriteLine("Podaj nazwę przepisu");
                //     string recipeName = Console.ReadLine();
                //     Console.WriteLine("Podaj ilość składników");
                //     int m = Convert.ToInt32(Console.ReadLine());
                //     List<Product> products = new List<Product>();
                //     for (int j = 0; j < m; j++)
                //     {
                //         Console.WriteLine("Podaj nazwę składnika");
                //         string productName = Console.ReadLine();
                //         Console.WriteLine("Podaj ilość składnika");
                //         double amount = Convert.ToDouble(Console.ReadLine());
                //         Console.WriteLine("Podaj jednostkę składnika");
                //         string unit = Console.ReadLine();
                //         Console.WriteLine("Podaj kategorię składnika");
                //         string category = Console.ReadLine();
                //         Console.WriteLine("Podaj ilość białka");
                //         double protein = Convert.ToDouble(Console.ReadLine());
                //         Console.WriteLine("Podaj ilość tłuszczu");
                //         double fat = Convert.ToDouble(Console.ReadLine());
                //         Console.WriteLine("Podaj ilość węglowodanów");
                //         double carbohydrates = Convert.ToDouble(Console.ReadLine());
                //         Product product = new Product(productName, unit, category, protein, fat, carbohydrates);
                //         products.Add(product);
                //     }
                //     Recipe recipe = new Recipe(recipeName, products);
                //     meal.Recipes.Add(recipe);
                // }
                // Console.WriteLine("Posiłek dodany");
                break;
        }
        }
    }

} 