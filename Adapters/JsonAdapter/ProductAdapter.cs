using System.Text.Json;
namespace Adapters.JsonAdapter{
public class ProductAdapter : IJsonAdapter<Product.Product>
{
    public Product.Product Deserialize(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
        {
            var productObject = doc.RootElement;
            var name = productObject.GetProperty("Name").GetString();
            var category = (Product.Product.Categories)Enum.Parse(typeof(Product.Product.Categories), productObject.GetProperty("Category").GetString());
            var unit = (Product.Product.Units)Enum.Parse(typeof(Product.Product.Units), productObject.GetProperty("Unit").GetString());
            var protein = productObject.GetProperty("Protein").GetDouble();
            var fat = productObject.GetProperty("Fat").GetDouble();
            var carbohydrates = productObject.GetProperty("Carbohydrates").GetDouble();

            return new Product.Product(name, unit, category, protein, fat, carbohydrates);
        }
    }

    public void Serialize(Utf8JsonWriter writer, Product.Product value, JsonSerializerOptions options)
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
}