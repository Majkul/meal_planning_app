using System.Xml.Serialization;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace Adapter{
public interface IJsonAdapter<T>
{
    T Deserialize(ref Utf8JsonReader reader, JsonSerializerOptions options);
    void Serialize(Utf8JsonWriter writer, T value, JsonSerializerOptions options);
}
public class IngredientListAdapter : IJsonAdapter<RecipeNamespace.IngredientList>
{
    public RecipeNamespace.IngredientList Deserialize(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        var ingredientList = new RecipeNamespace.IngredientList();

        using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
        {
            var ingredientsArray = doc.RootElement.EnumerateArray();

            foreach (var ingredientElement in ingredientsArray)
            {
                var productJson = ingredientElement.GetProperty("Product");
                var amount = ingredientElement.GetProperty("Amount").GetDouble();
                var product = JsonSerializer.Deserialize<Product>(productJson.GetRawText(), options);

                if (product != null) {
                    ingredientList.Add(product, amount);
                } else {
                    Console.WriteLine("Product could not be loaded.");
                    continue;
                }

            }
        }

        return ingredientList;
    }

    public void Serialize(Utf8JsonWriter writer, RecipeNamespace.IngredientList value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();

        foreach (var ingredient in value.Ingredients)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("Product");
            JsonSerializer.Serialize(writer, ingredient.Product, options);
            writer.WriteNumber("Amount", ingredient.Amount);
            writer.WriteEndObject();
        }

        writer.WriteEndArray();
    }
}
public class ProductAdapter : IJsonAdapter<Product>
{
    public Product Deserialize(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
        {
            var productObject = doc.RootElement;
            var name = productObject.GetProperty("Name").GetString();
            var category = (Product.Categories)Enum.Parse(typeof(Product.Categories), productObject.GetProperty("Category").GetString());
            var unit = (Product.Units)Enum.Parse(typeof(Product.Units), productObject.GetProperty("Unit").GetString());
            var protein = productObject.GetProperty("Protein").GetDouble();
            var fat = productObject.GetProperty("Fat").GetDouble();
            var carbohydrates = productObject.GetProperty("Carbohydrates").GetDouble();

            return new Product(name, unit, category, protein, fat, carbohydrates);
        }
    }

    public void Serialize(Utf8JsonWriter writer, Product value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString("Name", value.Name);
        writer.WriteString("Category", value.Category.ToString());
        writer.WriteString("Unit", value.Unit.ToString());
        writer.WriteNumber("Protein", value.Protein);
        writer.WriteNumber("Fat", value.Fat);
        writer.WriteNumber("Carbohydrates", value.Carbohydrates);
        writer.WriteEndObject();
    }
}
public class IngredientListConverter : JsonConverter<RecipeNamespace.IngredientList>
{
    private readonly IJsonAdapter<RecipeNamespace.IngredientList> _adapter = new IngredientListAdapter();

    public override RecipeNamespace.IngredientList Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return _adapter.Deserialize(ref reader, options);
    }

    public override void Write(Utf8JsonWriter writer, RecipeNamespace.IngredientList value, JsonSerializerOptions options)
    {
        _adapter.Serialize(writer, value, options);
    }
}

public class ProductConverter : JsonConverter<Product>
{
    private readonly IJsonAdapter<Product> _adapter = new ProductAdapter();

    public override Product Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return _adapter.Deserialize(ref reader, options);
    }

    public override void Write(Utf8JsonWriter writer, Product value, JsonSerializerOptions options)
    {
        _adapter.Serialize(writer, value, options);
    }
}
public interface IFileSaveAdapter
{
    void SaveToFile(string filePath, object data);
}

public class JsonFileSaveAdapter : IFileSaveAdapter
{
    public void SaveToFile(string filePath, object data)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters = { new Adapter.ProductConverter(), new Adapter.IngredientListConverter() }
        };

        var json = JsonSerializer.Serialize(data, options);
        File.WriteAllText(filePath, json);
    }
}

public class TxtFileSaveAdapter : IFileSaveAdapter
{
    public void SaveToFile(string filePath, object data)
    {
        var text = data.ToString();
        File.WriteAllText(filePath, text);
    }
}
public class FileSaveManager
{
    public void SaveToFile(string filePath, object data)
    {
        IFileSaveAdapter adapter = GetAdapter(filePath);
        adapter.SaveToFile(filePath, data);
    }

    private IFileSaveAdapter GetAdapter(string filePath)
    {
        var extension = Path.GetExtension(filePath).ToLower();

        return extension switch
        {
            ".json" => new JsonFileSaveAdapter(),
            ".txt" => new TxtFileSaveAdapter(),
            _ => throw new NotSupportedException($"File format '{extension}' is not supported."),
        };
    }
}
}