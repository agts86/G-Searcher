using System.Text.Json.Serialization;

namespace LineWebHookAPI.Constants.Line.API.Templates.Carousels;

/// <summary>
/// 画像のアスペクト比
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ImageAspectRatio
{
    /// <summary>
    /// 1.51:1
    /// </summary>
    Rectangle,

    /// <summary>
    /// 1:1
    /// </summary>
    Square
}
