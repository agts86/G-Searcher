using System.Text.Json;
using System.Text.Json.Serialization;
using LineWebHookAPI.Models.Dto.Line.Hook.Messages;
using LineWebHookAPI.Utilities;

namespace LineWebHookAPI.Jsons;

/// <summary>
/// インターフェイス型プロパティを実際の型に変換するJsonConverter
/// </summary>
/// <typeparam name="T"></typeparam>
public class MessageConverter : JsonConverter<Message>
{
    public override Message Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var backup = reader;
        using var doc = JsonDocument.ParseValue(ref reader);
        var typeValue = doc.RootElement.EnumerateObject()
            .Where(x => x.Name.Equals("type", StringComparison.OrdinalIgnoreCase))
            .Select(x => x.Value.GetString())
            .SingleOrDefault();
        var convertType = Polymorphism.CreatePolymorphismArray<Message>()
            .Where(x => x.Type == typeValue)
            .Select(x => x.GetType())
            .SingleOrDefault();
        if(convertType is null) return null;
        reader = backup;
        return JsonSerializer.Deserialize(ref reader, convertType, options) as Message;
    }


    public override void Write(Utf8JsonWriter writer, Message value, JsonSerializerOptions options)
    {
        var realType = value.GetType();
        JsonSerializer.Serialize(writer, value, realType, options);
    }
}
