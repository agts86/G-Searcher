using LineDevSdk.DTOs.WebHooks;

namespace LineWebHookAPI.Models.Dto.Yahoo;

/// <summary>
/// Yahoo!ローカルサーチジョブ
/// </summary>
public class YahooLocalJob(WebHook webHook, string genreCode)
{
    /// <summary>
    /// ジョブのID
    /// </summary>
    public string Id { get; } = Guid.NewGuid().ToString();

    /// <summary>
    /// WebHookの情報
    /// </summary>
    public WebHook WebHook { get; set; } = webHook;

    /// <summary>
    /// ジャンルコード
    /// </summary>
    public string GenreCode { get; } = genreCode;
}
