using System.Text.Json;
using System.Text.Json.Serialization;

namespace LineWebHookAPI.Jsons;

/// <summary>
/// インターフェイス型プロパティを実際の型に変換するJsonConverter
/// </summary>
/// <typeparam name="T"></typeparam>
public class RealMoldConverter<T> : JsonConverter<T>
{
    /// <summary>
    /// 読み取り
    /// 読み取りはそのまま
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="typeToConvert"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return JsonSerializer.Deserialize<T>(ref reader, options);
    }

    /// <summary>
    /// 書き込み
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="value"></param>
    /// <param name="options"></param>
    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        var realType = value.GetType();
        JsonSerializer.Serialize(writer, value, realType, options);
    }
}
