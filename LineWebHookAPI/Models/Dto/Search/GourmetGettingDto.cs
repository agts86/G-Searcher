using LineWebHookAPI.Models.Dto.Line.Messages;
using LineWebHookAPI.Models.Dto.Line;
using LineWebHookAPI.Models.Dto.Line.Sources;

namespace LineWebHookAPI.Models.Dto.HotPepper;

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
    public MessageEvent<UserSource,LocationMessage>[] Events {get; set;}
}
