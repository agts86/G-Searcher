using Application.Models.DB.Tables;

namespace Application.Models.DB.Repositories;

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
