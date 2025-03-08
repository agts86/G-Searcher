using LineWebHookAPI.Models.DB.Tables;
using LineWebHookAPI.Models.Dto.HotPepper;
using LineWebHookAPI.Models.DB.Daos;

namespace LineWebHookAPI.Models.DB.Repositories;

/// <summary>
/// サーチコントローラー用リポジトリ
/// </summary>
public class HotPepperRepository(MyContext dbContext) : Repository(dbContext)
{
    /// <summary>
    /// グルメログDAO
    /// </summary>
    public GourmetLogDao GourmetLogDao { get; } = new GourmetLogDao(dbContext);

    /// <summary>
    /// ログを作成する
    /// </summary>
    /// <param name="dto">リクエストデータ</param>
    public async Task CreateLogAsync(GourmetGettingDto dto)
    {
        var log = new GourmetLog
        {
            Id = Guid.NewGuid(),
            Lat = dto.Lat,
            Lng = dto.Lng
        };
        await GourmetLogDao.CreateLogAsync(log);
    }

    /// <summary>
    /// 保存する
    /// </summary>
    public async Task SaveChangesAsync()
    {
        await DbContext.SaveChangesAsync();
    }
}
