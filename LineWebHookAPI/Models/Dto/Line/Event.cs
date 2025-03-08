using System.ComponentModel.DataAnnotations;
using LineWebHookAPI.Models.Dto.Line.Sources;

namespace LineWebHookAPI.Models.Dto.Line;

/// <summary>
/// Webhookイベントオブジェクト
/// </summary>
public abstract class Event<TSource> where TSource : Source
{
    /// <summary>
    /// イベントのタイプ
    /// </summary>
    [Required]
    public string Type {get; set;}

    /// <summary>
    /// チャネルの状態
    /// </summary>
    [Required]
    public string Mode {get; set;}

    /// <summary>
    /// イベントの発生時刻
    /// </summary>
    public long Timestamp {get; set;}

    /// <summary>
    /// イベントの送信元情報を含むオブジェクト
    /// </summary>
    public TSource Source {get; set;}

    /// <summary>
    /// WebhookイベントID
    /// </summary>
    [Required]
    public string WebhookEventId {get; set;}

    /// <summary>
    /// Webhookイベントが再送されたものかどうか
    /// </summary>
    public DeliveryContextInfo DeliveryContext {get; set;}

    /// <summary>
    /// Webhookイベントが再送されたものかどうか
    /// </summary>
    public class DeliveryContextInfo
    {
        /// <summary>
        /// Webhookイベントが再送されたものかどうか
        /// </summary>
        public bool IsRedelivery {get; set;}
    }
}
