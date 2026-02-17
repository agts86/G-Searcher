using LineDevSdk.DTO.Commons.Messages;
using YahooDeveloperApiClient.YOLP.Request;

namespace Features.Yahoo.Extensions;

/// <summary>
/// LocalSearchRequest拡張クラス
/// </summary>
internal static class LocalSearchRequestExtensions
{
    /// <summary>
    /// メッセージ情報をマージします。
    /// </summary>
    /// <param name="dto">検索条件</param>
    /// <param name="message">送信メッセージ</param>
    /// <returns>検索条件</returns>
    public static void MergeMessageInfo(this LocalSearchRequest dto, IMessage message)
    {
        if (message is LocationMessage locationMessage)
        {
            dto.Lat = locationMessage.Latitude;
            dto.Lon = locationMessage.Longitude;
        }
        else if (message is TextMessage textMessage)
            dto.Query = textMessage.Text.Replace('　', ' ');
    }
}
