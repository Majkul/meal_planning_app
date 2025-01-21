using System.Text.Json;
namespace Adapters.JsonAdapter{
public class IngredientListAdapter : IJsonAdapter<Recipe.IngredientList>
{
    public Recipe.IngredientList Deserialize(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        var ingredientList = new Recipe.IngredientList();

        using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
        {
            var ingredientsArray = doc.RootElement.EnumerateArray();

            foreach (var ingredientElement in ingredientsArray)
            {
                var productJson = ingredientElement.GetProperty("Product");
                var amount = ingredientElement.GetProperty("Amount").GetDouble();
                var product = JsonSerializer.Deserialize<Product.Product>(productJson.GetRawText(), options);

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

    public void Serialize(Utf8JsonWriter writer, Recipe.IngredientList value, JsonSerializerOptions options)
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
}