using LineWebHookAPI.Models.DB.Tables;
using Microsoft.EntityFrameworkCore;

namespace LineWebHookAPI.Models.DB.Repositories;

public interface IManagedRepository
{
    /// <summary>
    /// ログを取得する
    /// </summary>
    Task<GourmetLocationLog[]> FetchGourmetLocationLogsAsync();
    
    /// <summary>
    /// ログを取得する
    /// </summary>
    Task<GourmetWordLog[]> FetchGourmetWordLogsAsync();

    /// <summary>
    /// エラーログを取得する
    /// </summary>
    /// <param name="ex">例外</param>
    Task<ErrorLog[]> FetchErrorLogsAsync();
}

/// <summary>
/// サーチコントローラー用リポジトリ
/// </summary>
public class ManagedRepository(LineWebHookContext dbContext) : IManagedRepository
{
    /// <summary>
    /// EFCoreのコンテキスト
    /// </summary>
    private LineWebHookContext DbContext { get; } = dbContext;

    /// <summary>
    /// ログを取得する
    /// </summary>
    /// <returns>ログ</returns>
    public async Task<GourmetLocationLog[]> FetchGourmetLocationLogsAsync()
    {
        return await DbContext.GourmetLocationLogs.AsNoTracking()
            .OrderBy(x => x.CreatedAt)
            .ToArrayAsync();
    }

    /// <summary>
    /// ログを取得する
    /// </summary>
    /// <returns>ログ</returns>
    public async Task<GourmetWordLog[]> FetchGourmetWordLogsAsync()
    {
        return await DbContext.GourmetWordLogs.AsNoTracking()
            .OrderBy(x => x.CreatedAt)
            .ToArrayAsync();
    }

    /// <summary>
    /// ログを取得する
    /// </summary>
    public async Task<ErrorLog[]> FetchErrorLogsAsync()
    {
        return await DbContext.ErrorLogs.AsNoTracking()
            .OrderBy(x => x.CreatedAt)
            .ToArrayAsync();
    }
}
