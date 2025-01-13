using System.Text.Json;
using System.Text.Json.Serialization;
public class Product {
    public enum Categories { Fruit, Vegetable, Meat, Dairy, Grain, Other }
    public enum Units { Grams, Milliliters, Units }
    public string Name { get; set; }
    public Categories Category { get; set; }
    public Units Unit { get; set; }
    public double Protein { get; set; }
    public double Fat { get; set; }
    public double Carbohydrates { get; set; }

    public Product(string name, Units unit, Categories category, double protein, double fat, double carbohydrates) {
        Name = name;
        Unit = unit;
        Category = category;
        Protein = protein;
        Fat = fat;
        Carbohydrates = carbohydrates;
    }
    public override string ToString() {
        return $"{Name} ({Category})";
    }

    public class ProductConverter : JsonConverter<Product>
{
    public override Product Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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

    public override void Write(Utf8JsonWriter writer, Product value, JsonSerializerOptions options)
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
