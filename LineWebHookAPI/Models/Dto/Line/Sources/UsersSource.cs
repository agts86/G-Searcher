using System.ComponentModel.DataAnnotations;

namespace LineWebHookAPI.Models.Dto.Line.Sources;

/// <summary>
/// 送信元複数人トーク情報
/// </summary>
public class UsersSource : UserSource
{
    /// <summary>
    /// 送信元複数人トークのトークルームID
    /// </summary>
    [Required]
    public string RoomId { get; set; }

    /// <summary>
    /// 送信元ユーザーのID
    /// </summary>
    public override string UserId { get; set; }
}
