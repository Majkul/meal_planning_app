using System.Globalization;
namespace Services{
public class UIHandler{
    public static void CalculateAndDisplaySummary(Meal.MealHistory mealHistory, DateTime today, string[] meals) {
        int TotalCalories = 0;
        double TotalProtein = 0;
        double TotalFat = 0;
        double TotalCarbohydrates = 0;

        List<string?> meals_names_in_date = MealServices.GetMealsNamesForDate(mealHistory, today);
        List<Meal.Meal.MealMemento> meals_in_date = MealServices.GetMealsForDate(mealHistory, today);
        for(int i=0; i<5; i++)
        {
            int TotalRecipeCalories = 0;
            double TotalRecipeProtein = 0;
            double TotalRecipeFat = 0;
            double TotalRecipeCarbohydrates = 0;
            Console.Write("|");
            Console.Write(meals[i].PadLeft((111 - meals[i].Length) / 2 + meals[i].Length).PadRight(111));
            Console.WriteLine("|");

            string mealName = meals_names_in_date[i] ?? "No meal";
            if(mealName!="No meal"){
                foreach (var recipe in meals_in_date[i].Recipes) {
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

        Console.Write("|");
        string summaryAllDay = $"Total:   Kcal: {TotalCalories}, Prot.: {TotalProtein}g, Fats: {TotalFat}g, Carbs: {TotalCarbohydrates}g";
        Console.Write(summaryAllDay.PadLeft((111 - summaryAllDay.Length) / 2 + summaryAllDay.Length).PadRight(111));
        Console.WriteLine("|");
        Console.WriteLine(new string('-', 113));
    }
}
}