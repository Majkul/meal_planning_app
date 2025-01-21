using System.Text.Json;
using System.Text.Json.Serialization;
namespace Adapters.JsonAdapter{
public class ProductConverter : JsonConverter<Product.Product>
{
    private readonly IJsonAdapter<Product.Product> _adapter = new ProductAdapter();

    public override Product.Product Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return _adapter.Deserialize(ref reader, options);
    }

    public override void Write(Utf8JsonWriter writer, Product.Product value, JsonSerializerOptions options)
    {
        _adapter.Serialize(writer, value, options);
    }
}
}