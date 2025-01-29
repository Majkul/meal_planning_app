using ShoppingList.Decorators;
namespace Services{
public static class ShoppingListServices{
    public static void PickDate(Meal.MealHistory mealHistory, DB.DatabaseConnection<Product.Product> ProductsDatabase){
        Console.WriteLine("Wybierz zakres dat (format: dd/MM/yyyy, np. 22.01.2025):");
        DateTime startDate = VarServices.GetDateFromString("Od: ");
        DateTime endDate = VarServices.GetDateFromString("Do: ");
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
    }
    
    public static void OnShopping(ShoppingList.ShoppingList shoppingList, DB.DatabaseConnection<Product.Product> ProductsDatabase){
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
}
}