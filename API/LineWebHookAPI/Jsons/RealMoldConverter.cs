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
        return JsonSerializer.Deserialize<T>(ref reader, options);
    }


    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        var realType = value.GetType();
        JsonSerializer.Serialize(writer, value, realType, options);
    }
}
