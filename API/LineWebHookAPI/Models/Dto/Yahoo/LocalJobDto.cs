using System.Text.Json;
using LineDevSdk.DTOs.WebHooks;

namespace LineWebHookAPI.Models.Dto.Yahoo;

/// <summary>
/// Yahoo!ローカルサーチジョブ
/// </summary>
public class LocalJobDto(WebHook webHook, string genreCode)
{
    /// <summary>
    /// ジョブのID
    /// </summary>
    public Guid Id { get; } = Guid.NewGuid();

    /// <summary>
    /// WebHookの情報
    /// </summary>
    public WebHook WebHook { get; } = webHook;

    /// <summary>
    /// ジャンルコード
    /// </summary>
    public string GenreCode { get; } = genreCode;

    public string GetBody() => JsonSerializer.Serialize(this);
}
