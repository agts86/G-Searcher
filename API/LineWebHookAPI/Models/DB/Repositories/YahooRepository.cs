using LineWebHookAPI.Models.DB.Tables;
using LineWebHookAPI.Models.DB.Daos;
using LineWebHookAPI.Models.Dto.Line.Hook.Messages;

namespace LineWebHookAPI.Models.DB.Repositories;

/// <summary>
/// Yahooコントローラー用リポジトリ
/// </summary>
public class YahooRepository(LineWebHookContext dbContext) : Repository(dbContext)
{
    /// <summary>
    /// グルメログDAO
    /// </summary>
    public GourmetLogDao GourmetLogDao { get; } = new GourmetLogDao(dbContext);

    /// <summary>
    /// ログを作成する
    /// </summary>
    /// <param name="dto">位置情報メッセージ</param>
    public async Task CreateLogAsync(LocationMessage message)
    {
        var log = new GourmetLog
        {
            Id = Guid.NewGuid(),
            Lat = message.Latitude,
            Lng = message.Longitude
        };
        await GourmetLogDao.CreateLogAsync(log);
    }
}
