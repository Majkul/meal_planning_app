using System.Text.Json;
namespace Adapters.FileSaveAdapter
{
public class JsonFileSaveAdapter : IFileSaveAdapter
{
    public void SaveToFile(string filePath, object data)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters = { new Adapters.JsonAdapter.ProductConverter(), new Adapters.JsonAdapter.IngredientListConverter() }
        };

        var json = JsonSerializer.Serialize(data, options);
        File.WriteAllText(filePath, json);
    }
}
}