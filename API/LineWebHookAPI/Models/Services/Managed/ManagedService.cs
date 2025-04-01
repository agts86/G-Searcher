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
    Task<GourmetLog[]> GetGourmetLogAsync();

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
    public async Task<GourmetLog[]> GetGourmetLogAsync()
    {
        return await ManagedRepository.FetchGourmetLogAsync();
    }

    /// <summary>
    /// エラーログを取得する
    /// </summary>
    /// <returns>エラーログ</returns>
    public async Task<ErrorLog[]> GetErrorLogAsync()
    {
        return await ManagedRepository.FetchErrorLogAsync();
    }
}
