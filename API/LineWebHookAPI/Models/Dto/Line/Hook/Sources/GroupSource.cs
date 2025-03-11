using System.ComponentModel.DataAnnotations;

namespace LineWebHookAPI.Models.Dto.Line.Hook.Sources;

/// <summary>
/// 送信元グループ情報
/// </summary>
public class GroupSource : UserSource
{
    /// <summary>
    /// 送信元グループのID
    /// </summary>
    [Required]
    public string GroupId { get; set; }

    /// <summary>
    /// 送信元ユーザーのID
    /// </summary>
    public override string UserId { get; set; }
}
