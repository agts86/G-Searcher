using LineWebHookAPI.Models.DB.Tables;
using Microsoft.EntityFrameworkCore;

namespace LineWebHookAPI.Models.DB.Repositories;

public abstract class ManagedRepositoryBase(LineWebHookContext dbContext) : Repository(dbContext)
{
    /// <summary>
    /// エラーログを作成する
    /// </summary>
    /// <param name="ex">例外</param>
    public abstract Task<GourmetLog[]> FetchGourmetLogAsync();

    /// <summary>
    /// エラーログを作成する
    /// </summary>
    /// <param name="ex">例外</param>
    public abstract Task<ErrorLog[]> FetchErrorLogAsync();
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
    public override async Task<GourmetLog[]> FetchGourmetLogAsync()
    {
        return await DbContext.GourmetLogs.AsNoTracking().ToArrayAsync();
    }

    /// <summary>
    /// ログを取得する
    /// </summary>
    public override async Task<ErrorLog[]> FetchErrorLogAsync()
    {
        return await DbContext.ErrorLogs.AsNoTracking().ToArrayAsync();
    }
}
