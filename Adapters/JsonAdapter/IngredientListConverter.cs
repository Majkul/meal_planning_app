using System.Text.Json;
using System.Text.Json.Serialization;
namespace Adapters.JsonAdapter{
public class IngredientListConverter : JsonConverter<Recipe.IngredientList>
{
    private readonly IJsonAdapter<Recipe.IngredientList> _adapter = new IngredientListAdapter();

    public override Recipe.IngredientList Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return _adapter.Deserialize(ref reader, options);
    }

    public override void Write(Utf8JsonWriter writer, Recipe.IngredientList value, JsonSerializerOptions options)
    {
        _adapter.Serialize(writer, value, options);
    }
}
}