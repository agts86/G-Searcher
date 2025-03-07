using G_Searcher.Models.DB.Daos;

namespace G_Searcher.Models.DB.Repositories;

/// <summary>
/// ミドルウェア用リポジトリ
/// </summary>
public class MiddleWareRepository(MyContext dbContext) : Repository(dbContext)
{
    /// <summary>
    /// エラーログDAO
    /// </summary>
    public ErrorLogDao ErrorLogDao { get; } = new ErrorLogDao(dbContext);

    /// <summary>
    /// 保存する
    /// </summary>
    public async Task SaveChangesAsync()
    {
        await DbContext.SaveChangesAsync();
    }
}
