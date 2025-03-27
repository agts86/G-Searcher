using System.ComponentModel.DataAnnotations;

namespace LineWebHookAPI.Models.Dto.Line.Hook.Messages;

/// <summary>
/// メッセージ基底クラス
/// </summary>
public abstract class Message
{
    /// <summary>
    /// メッセージID
    /// </summary>
    [Required]
    public string Id { get; set; }

    /// <summary>
    /// タイプ
    /// </summary>
    public abstract string Type { get; }

    /// <summary>
    /// HotPepperAPIのクエリを作成する
    /// </summary>
    public abstract string CreateHotPepperApiQuey();
}
