using LineWebHookAPI.Models.DB.Repositories;
using LineWebHookAPI.Models.DB;
using LineWebHookAPI.Models.DB.Tables;

namespace LineWebHookAPI.Models.BL.Managed;

/// <summary>
/// 管理用コントローラーのビジネスロジック
/// </summary>
public class ManagedBL(LineWebHookContext dbContext)
{
    /// <summary>
    /// リポジトリ
    /// </summary>
    public ManagedRepository ManagedRepository { get; protected set; } = new ManagedRepository(dbContext);

    /// <summary>
    /// LineChatBotの実行ログを取得する
    /// </summary>
    /// <returns>LineChatBotの実行ログ</returns>
    public async Task<GourmetLog[]> GetGourmetLogAsync()
    {
        return await ManagedRepository.GourmetLogDao.FetchLogAsync();
    }

    /// <summary>
    /// エラーログを取得する
    /// </summary>
    /// <returns>エラーログ</returns>
    public async Task<ErrorLog[]> GetErrorLogAsync()
    {
        return await ManagedRepository.ErrorLogDao.FetchLogAsync();
    }
}
