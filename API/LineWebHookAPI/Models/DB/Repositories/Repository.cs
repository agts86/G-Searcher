using LineWebHookAPI.Models.DB.Tables;
using LineWebHookAPI.Models.Exceptions;

namespace LineWebHookAPI.Models.DB.Repositories;

/// <summary>
/// リポジトリの基底クラス
/// </summary>
public abstract class Repository(LineWebHookContext dbContext)
{
    /// <summary>
    /// EFCoreのコンテキスト
    /// </summary>
    protected LineWebHookContext DbContext { get; } = dbContext;

    /// <summary>
    /// 保存する
    /// </summary>
    public async Task SaveChangesAsync()
    {
        await DbContext.SaveChangesAsync();
    }
}
