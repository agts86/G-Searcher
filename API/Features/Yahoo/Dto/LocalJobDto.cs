using System.Text.Json;
using LineDevSdk.DTO.WebHooks;

namespace Features.Yahoo.Dto;

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
