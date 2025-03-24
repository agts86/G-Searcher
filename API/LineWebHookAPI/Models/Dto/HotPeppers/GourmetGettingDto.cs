using LineWebHookAPI.Jsons;
using LineWebHookAPI.Models.Dto.Line.Hook.Messages;
using LineWebHookAPI.Models.Dto.Line.Hook.Sources;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LineWebHookAPI.Models.Dto.HotPeppers;

/// <summary>
/// PostGourmetLocationAsyncのリクエストDto
/// </summary>
public class GourmetGettingDto
{
    /// <summary>
    /// Webhookイベントを受信すべきボットのユーザーID
    /// </summary>
    public string Destination {get; set;}

    /// <summary>
    /// Webhookイベントオブジェクトの配列
    /// </summary>
    public GourmetEvent[] Events {get; set;} = [];

    /// <summary>
    /// Webhookイベントオブジェクト
    /// </summary>
    public class GourmetEvent
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
        public UserSource Source {get; set;}

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

        /// <summary>
        /// 応答トークン
        /// </summary>
        [Required]
        public string ReplyToken {get; set;}

        /// <summary>
        /// メッセージの内容を含むオブジェクト
        /// </summary>
        [Required]
        [JsonConverter(typeof(MessageConverter))]
        public Message Message {get; set;}
    }
}
