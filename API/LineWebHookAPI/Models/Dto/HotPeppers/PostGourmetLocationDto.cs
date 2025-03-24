using LineWebHookAPI.Constants.HotPepper;
using LineWebHookAPI.Models.Dto.Line.Hook.Messages;

namespace LineWebHookAPI.Models.Dto.HotPeppers;

/// <summary>
/// PostGourmetLocationAsyncのリクエストDto
/// </summary>
public class PostGourmetLocationDto(GourmetGettingDto gourmetGettingDto, GenreCode genreCode)
{
    /// <summary>
    /// 位置情報メッセージ
    /// </summary>
    public LocationMessage Message { get; } = gourmetGettingDto.Events.Select(x => x.Message).FirstOrDefault();

    /// <summary>
    /// 返信用トークン
    /// </summary>
    public string ReplyToken { get; } = gourmetGettingDto.Events.Select(x => x.ReplyToken).FirstOrDefault();

    /// <summary>
    /// ジャンルコード
    /// </summary>
    public GenreCode GenreCode { get; } = genreCode;
}
