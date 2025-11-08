using LineWebHookAPI.Constants;
using LineWebHookAPI.Models.DB.Repositories;
using LineWebHookAPI.Models.DB.Tables;

namespace LineWebHookAPI.Models.Services;

public interface IManagedService
{
    /// <summary>
    /// LineChatBotの実行ログを取得する
    /// </summary>
    /// <returns>LineChatBotの実行ログ</returns>
    Task<Meta[]> GetGourmetLogsAsync(MessageTypes messageType);

    /// <summary>
    /// エラーログを取得する
    /// </summary>
    /// <returns>エラーログ</returns>
    Task<ErrorLog[]> GetErrorLogAsync();

    /// <summary>
    /// ジョブログを取得する
    /// </summary>
    /// <returns>ジョブログ</returns>
    Task<JobLog[]> GetJobLogAsync();
}

/// <summary>
/// 管理用コントローラーのビジネスロジック
/// </summary>
public class ManagedService(IManagedRepository managedRepository) : IManagedService
{
    /// <summary>
    /// リポジトリ
    /// </summary>
    private IManagedRepository ManagedRepository { get; } = managedRepository;

    /// <summary>
    /// LineChatBotの実行ログを取得する
    /// </summary>
    /// <returns>LineChatBotの実行ログ</returns>
    public async Task<Meta[]> GetGourmetLogsAsync(MessageTypes messageType)
    {
        if (messageType == MessageTypes.location)
            return await ManagedRepository.FetchGourmetLocationLogsAsync();
        return await ManagedRepository.FetchGourmetWordLogsAsync();
    }

    /// <summary>
    /// エラーログを取得する
    /// </summary>
    /// <returns>エラーログ</returns>
    public async Task<ErrorLog[]> GetErrorLogAsync()
    {
        return await ManagedRepository.FetchErrorLogsAsync();
    }

    /// <summary>
    /// ジョブログを取得する
    /// </summary>
    /// <returns>ジョブログ</returns>
    public async Task<JobLog[]> GetJobLogAsync()
    {
        return await ManagedRepository.FetchJobLogAsync();
    }
}
