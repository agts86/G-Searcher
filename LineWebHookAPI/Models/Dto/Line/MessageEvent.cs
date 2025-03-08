using System.ComponentModel.DataAnnotations;
using LineWebHookAPI.Models.Dto.Line.Sources;
using LineWebHookAPI.Models.Dto.Line.Messages; // Add this line to import the Message type

namespace LineWebHookAPI.Models.Dto.Line;

/// <summary>
/// Webhookイベントオブジェクト
/// </summary>
public class MessageEvent<TSource,TMessage> : Event<TSource> where TSource : Source where TMessage : Message
{
    /// <summary>
    /// 応答トークン
    /// </summary>
    [Required]
    public string ReplyToken {get; set;}

    /// <summary>
    /// メッセージの内容を含むオブジェクト
    /// </summary>
    [Required]
    public TMessage Message {get; set;}
}
