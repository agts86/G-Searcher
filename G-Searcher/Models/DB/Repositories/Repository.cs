namespace G_Searcher.Models.DB.Repositories;

/// <summary>
/// リポジトリの基底クラス
/// </summary>
public abstract class Repository(MyContext dbContext)
{
    /// <summary>
    /// EFCoreのコンテキスト
    /// </summary>
    protected MyContext DbContext { get; } = dbContext;
}
