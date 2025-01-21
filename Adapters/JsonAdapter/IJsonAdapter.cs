using System.Text.Json;
namespace Adapters.JsonAdapter
{
public interface IJsonAdapter<T>
{
    T Deserialize(ref Utf8JsonReader reader, JsonSerializerOptions options);
    void Serialize(Utf8JsonWriter writer, T value, JsonSerializerOptions options);
}
}