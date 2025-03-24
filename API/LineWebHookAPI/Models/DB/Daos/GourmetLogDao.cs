using LineWebHookAPI.Models.DB.Tables;
using Microsoft.EntityFrameworkCore;

namespace LineWebHookAPI.Models.DB.Daos;

/// <summary>
/// グルメログDAO
/// </summary>
public class GourmetLogDao(LineWebHookContext dbContext) : Dao(dbContext)
{
    /// <summary>
    /// ログを作成する
    /// </summary>
    /// <param name="dto">登録データ</param>
    public async Task CreateLogAsync(GourmetLog log)
    {
        await DbContext.GourmetLogs.AddAsync(log);
    }

    /// <summary>
    /// ログを取得する
    /// </summary>
    /// <returns>ログ</returns>
    public async Task<GourmetLog[]> FetchLogAsync()
    {
        return await DbContext.GourmetLogs.AsNoTracking().ToArrayAsync();
    }
}
