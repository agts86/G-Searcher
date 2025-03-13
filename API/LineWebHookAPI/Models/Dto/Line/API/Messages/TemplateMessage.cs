using System.Text.Json.Serialization;
using LineWebHookAPI.Models.Dto.Line.API.Messages.Templates;
using LineWebHookAPI.Jsons;

namespace LineWebHookAPI.Models.Dto.Line.API.Messages;

/// <summary>
/// テンプレートメッセージ
/// </summary>
public class TemplateMessage : IMessage
{
    /// <summary>
    /// タイプ
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; } = "template";

    /// <summary>
    /// テキスト
    /// </summary>
    [JsonPropertyName("altText")]
    public string AltText { get; set; }

    /// <summary>
    /// テンプレート
    /// </summary>
    [JsonConverter(typeof(RealMoldConverter<ITemplate>))]
    [JsonPropertyName("template")]
    public ITemplate Template { get; set; }
}
