using LineWebHookAPI.Constants;
using LineWebHookAPI.Models.DB.Repositories;
using LineWebHookAPI.Models.DB.Tables;

namespace LineWebHookAPI.Models.Services.Managed;

/// <summary>
/// 管理用コントローラーのビジネスロジック
/// </summary>
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
}

/// <summary>
/// 管理用コントローラーのビジネスロジック
/// </summary>
public class ManagedService(ManagedRepositoryBase managedRepository) : IManagedService
{
    /// <summary>
    /// リポジトリ
    /// </summary>
    public ManagedRepositoryBase ManagedRepository { get; protected set; } = managedRepository;

    /// <summary>
    /// LineChatBotの実行ログを取得する
    /// </summary>
    /// <returns>LineChatBotの実行ログ</returns>
    public async Task<Meta[]> GetGourmetLogsAsync(MessageTypes messageType)
    {
        if(messageType == MessageTypes.location)
            return await ManagedRepository.FetchGourmetLocationLogsAsync();
        else if (messageType == MessageTypes.text)
            return await ManagedRepository.FetchGourmetWordLogsAsync();
        return [];
    }

    /// <summary>
    /// エラーログを取得する
    /// </summary>
    /// <returns>エラーログ</returns>
    public async Task<ErrorLog[]> GetErrorLogAsync()
    {
        return await ManagedRepository.FetchErrorLogsAsync();
    }
}
