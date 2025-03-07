namespace G_Searcher.Models.DB.Daos;

/// <summary>
/// Daoの基底クラス
/// </summary>
public abstract class Dao(MyContext dbContext)
{
    /// <summary>
    /// EFCoreのコンテキスト
    /// </summary>
    protected MyContext DbContext { get; } = dbContext;
}
