using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using LineWebHookAPI.Jsons;
using LineWebHookAPI.Models.Dto.Line.API.Messages;

namespace LineWebHookAPI.Models.Dto.Line.API.Requests;

/// <summary>
/// 応答メッセージ
/// </summary>
public class Replay
{
    /// <summary>
    /// 応答トークン
    /// </summary>
    [Required]
    public string ReplyToken {get; set;}

    /// <summary>
    /// メッセージ
    /// </summary>
    [JsonConverter(typeof(RealMoldConverter<IMessage[]>))]
    public IMessage[] Messages {get; set;}
}
