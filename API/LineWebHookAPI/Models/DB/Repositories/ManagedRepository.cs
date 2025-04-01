using LineWebHookAPI.Models.DB.Tables;
using Microsoft.EntityFrameworkCore;

namespace LineWebHookAPI.Models.DB.Repositories;

public abstract class ManagedRepositoryBase(LineWebHookContext dbContext) : Repository(dbContext)
{
    /// <summary>
    /// ログを取得する
    /// </summary>
    public abstract Task<GourmetLocationLog[]> FetchGourmetLocationLogsAsync();
    
    /// <summary>
    /// ログを取得する
    /// </summary>
    public abstract Task<GourmetWordLog[]> FetchGourmetWordLogsAsync();

    /// <summary>
    /// エラーログを作成する
    /// </summary>
    /// <param name="ex">例外</param>
    public abstract Task<ErrorLog[]> FetchErrorLogsAsync();
}

/// <summary>
/// サーチコントローラー用リポジトリ
/// </summary>
public class ManagedRepository(LineWebHookContext dbContext) : ManagedRepositoryBase(dbContext)
{
    /// <summary>
    /// ログを取得する
    /// </summary>
    /// <returns>ログ</returns>
    public override async Task<GourmetLocationLog[]> FetchGourmetLocationLogsAsync()
    {
        return await DbContext.GourmetLocationLogs.AsNoTracking().ToArrayAsync();
    }

    /// <summary>
    /// ログを取得する
    /// </summary>
    /// <returns>ログ</returns>
    public override async Task<GourmetWordLog[]> FetchGourmetWordLogsAsync()
    {
        return await DbContext.GourmetWordLogs.AsNoTracking().ToArrayAsync();
    }

    /// <summary>
    /// ログを取得する
    /// </summary>
    public override async Task<ErrorLog[]> FetchErrorLogsAsync()
    {
        return await DbContext.ErrorLogs.AsNoTracking().ToArrayAsync();
    }
}
