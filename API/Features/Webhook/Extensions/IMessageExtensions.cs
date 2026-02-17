using LineDevSdk.DTO.Commons.Messages;
using Application.Models.DB.Tables;

namespace Features.Webhook.Extensions;

/// <summary>
/// IMessageの拡張メソッド
/// </summary>
internal static class IMessageExtensions
{
    /// <summary>
    /// IMessageをGourmetLogに変換する
    /// </summary>
    /// <param name="message">変換するメッセージ</param>
    /// <returns>変換後のGourmetLog</returns>
    public static Meta ConvertGourmetLog(this IMessage message)
    {
        Meta log = null;
        if (message is LocationMessage locationMessage)
        {
            log = new GourmetLocationLog
            {
                Id = Guid.NewGuid(),
                Lat = locationMessage.Latitude,
                Lng = locationMessage.Longitude
            };
        }
        else if (message is TextMessage textMessage)
        {
            log = new GourmetWordLog
            {
                Id = Guid.NewGuid(),
                Text = textMessage.Text
            };
        }
        return log;
    }
}
