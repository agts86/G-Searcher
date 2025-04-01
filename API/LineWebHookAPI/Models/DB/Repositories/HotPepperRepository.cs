using LineWebHookAPI.Models.DB.Tables;
using LineWebHookAPI.Models.Dto.Line.Hook.Messages;

namespace LineWebHookAPI.Models.DB.Repositories;

/// <summary>
/// HotPepperコントローラー用リポジトリ
/// </summary>
public abstract class HotPepperRepositoryBase(LineWebHookContext dbContext) : Repository(dbContext)
{
    /// <summary>
    /// ログを作成する
    /// </summary>
    /// <param name="dto">位置情報メッセージ</param>
    public abstract Task CreateGourmetLogAsync(LocationMessage message);
}

/// <summary>
/// HotPepperコントローラー用リポジトリ
/// </summary>
public class HotPepperRepository(LineWebHookContext dbContext) : HotPepperRepositoryBase(dbContext)
{
    /// <summary>
    /// ログを作成する
    /// </summary>
    /// <param name="dto">位置情報メッセージ</param>
    public override async Task CreateGourmetLogAsync(LocationMessage message)
    {
        var log = new GourmetLog
        {
            Id = Guid.NewGuid(),
            Lat = message.Latitude,
            Lng = message.Longitude
        };
        await DbContext.GourmetLogs.AddAsync(log);
    }
}
