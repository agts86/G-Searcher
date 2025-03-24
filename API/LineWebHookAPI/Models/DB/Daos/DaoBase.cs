namespace LineWebHookAPI.Models.DB.Daos;

/// <summary>
/// Daoの基底クラス
/// </summary>
public abstract class Dao(LineWebHookContext dbContext)
{
    /// <summary>
    /// EFCoreのコンテキスト
    /// </summary>
    protected LineWebHookContext DbContext { get; } = dbContext;
}
