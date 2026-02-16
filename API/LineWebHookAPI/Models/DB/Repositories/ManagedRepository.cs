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

    Task<JobLog[]> FetchJobLogAsync();
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
    /// 位置情報検索ログを取得する
    /// </summary>
    /// <returns>ログ</returns>
    public async Task<GourmetLocationLog[]> FetchGourmetLocationLogsAsync()
    {
        return await DbContext.GourmetLocationLogs.AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .ToArrayAsync();
    }

    /// <summary>
    /// 文字列検索ログを取得する
    /// </summary>
    /// <returns>ログ</returns>
    public async Task<GourmetWordLog[]> FetchGourmetWordLogsAsync()
    {
        return await DbContext.GourmetWordLogs.AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .ToArrayAsync();
    }

    /// <summary>
    /// エラーログを取得する
    /// </summary>
    /// <returns>ログ</returns>
    public async Task<ErrorLog[]> FetchErrorLogsAsync()
    {
        return await DbContext.ErrorLogs.AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .ToArrayAsync();
    }

    /// <summary>
    /// ジョブログを取得する
    /// </summary>
    /// <returns></returns>
    public async Task<JobLog[]> FetchJobLogAsync()
    {
        return await DbContext.JobLogs.AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .ToArrayAsync();
    }
}
