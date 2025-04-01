using System.Text.Json;
using System.Text.Json.Serialization;

namespace LineWebHookAPI.Jsons;

/// <summary>
/// インターフェイス型プロパティを実際の型に変換するJsonConverter
/// </summary>
/// <typeparam name="T"></typeparam>
public class RealMoldConverter<T> : JsonConverter<T>
{
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }


    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        if(value is Array array)
        {
            writer.WriteStartArray();
            foreach (var item in array)
                JsonSerializer.Serialize(writer, item, item.GetType(), options);
            writer.WriteEndArray();
        }
        else
            JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}
