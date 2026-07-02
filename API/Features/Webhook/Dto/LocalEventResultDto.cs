using LineDevSdk.DTO.MessagingAPIs;
using Application.Models.DB.Tables;

namespace Features.Webhook.Dto;

/// <summary>
/// ローカル検索イベント1件分の処理結果
/// </summary>
public class LocalEventResultDto(Reply reply, Meta meta)
{
    /// <summary>
    /// 返信内容
    /// </summary>
    public Reply Reply { get; } = reply;

    /// <summary>
    /// 登録するログ
    /// </summary>
    public Meta Meta { get; } = meta;

    /// <summary>
    /// 返信が成功したか
    /// </summary>
    public bool IsReplySucceeded { get; set; }
}
