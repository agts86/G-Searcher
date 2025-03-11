using System.ComponentModel.DataAnnotations;

namespace LineWebHookAPI.Models.Dto.Line.Hook.Sources;

/// <summary>
/// 送信元ユーザー情報
/// </summary>
public class UserSource : Source
{
    /// <summary>
    /// 送信元ユーザーのID
    /// </summary>
    [Required]
    public override string UserId { get; set; }
}
