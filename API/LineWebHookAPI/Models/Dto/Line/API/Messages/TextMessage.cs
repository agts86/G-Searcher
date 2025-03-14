using System.Text.Json.Serialization;

namespace LineWebHookAPI.Models.Dto.Line.API.Messages;

/// <summary>
/// テキストメッセージ
/// </summary>
public class TextMessage : IMessage
{
    /// <summary>
    /// タイプ
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; } = "text";

    /// <summary>
    /// テキスト
    /// </summary>
    [JsonPropertyName("text")]
    public string Text { get; set; }
}
