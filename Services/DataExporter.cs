namespace Services{
public class DataExporter{
    public static void ExportData(Meal.MealHistory mealHistory) {
        Console.WriteLine("Wybierz format eksportu (json/txt):");
        string exportFormat = Console.ReadLine()?.ToLower()!;

        var fileManager = new Adapters.FileSaveAdapter.FileSaveManager();
        switch (exportFormat)
        {
            case "json":
                fileManager.SaveToFile($"export-{DateTime.Today:dd.MM.yyyy-HH.mm.ss}.json", mealHistory);
                Console.WriteLine("Historia posiłków została wyeksportowana do formatu JSON.");
                break;
            case "txt":
                fileManager.SaveToFile($"export-{DateTime.Today:dd.MM.yyyy-HH.mm.ss}.txt", mealHistory);
                Console.WriteLine("Historia posiłków została wyeksportowana do formatu TXT.");
                break;
            default:
                Console.WriteLine("Nieprawidłowy format eksportu.\n");
                break;
        }
    }
}
}