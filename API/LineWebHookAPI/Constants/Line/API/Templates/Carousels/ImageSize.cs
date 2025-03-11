using System.Text.Json.Serialization;

namespace LineWebHookAPI.Constants.Line.API.Templates.Carousels;

/// <summary>
/// 画像の表示形式
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ImageSize
{
    /// <summary>
    /// 画像領域に収まらない部分は切り詰められます。
    /// </summary>
    Cover,

    /// <summary>
    /// 縦長の画像では左右に、横長の画像では上下に余白が表示されます。
    /// </summary>
    Contain
}
