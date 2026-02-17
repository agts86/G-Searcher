using Application.Models.DB.Tables;
using Features.Managed.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Models.DB.Repositories;

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
