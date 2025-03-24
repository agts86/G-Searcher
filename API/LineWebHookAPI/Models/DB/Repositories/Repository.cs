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
}
