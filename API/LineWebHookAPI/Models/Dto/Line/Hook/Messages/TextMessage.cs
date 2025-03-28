using System.ComponentModel.DataAnnotations;

namespace LineWebHookAPI.Models.Dto.Line.Hook.Messages;

/// <summary>
/// テキストメッセージ
/// </summary>
public class TextMessage : Message
{
    /// <summary>
    /// タイプ
    /// </summary>
    [Required]
    public override string Type { get; } = "text";

    /// <summary>
    /// テキスト
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// HotPepperAPIのクエリを作成する
    /// </summary>
    public override string CreateHotPepperApiQuey()
    {
        var value = Text.Replace('　', ' ');
        return $"&keyword={Uri.EscapeDataString(value)}";
    } 

    /// <summary>
    /// YahooAPIのクエリを作成する
    /// </summary>
    public override string CreateYahooApiQuey()
    {
        var value = Text.Replace('　', ' ');
        return $"&query={Uri.EscapeDataString(value)}";
    } 
}
