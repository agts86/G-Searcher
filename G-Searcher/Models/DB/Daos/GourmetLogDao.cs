using G_Searcher.Models.DB.Tables;

namespace G_Searcher.Models.DB.Daos;

/// <summary>
/// グルメログDAO
/// </summary>
public class GourmetLogDao(MyContext dbContext) : Dao(dbContext)
{
    /// <summary>
    /// ログを作成する
    /// </summary>
    /// <param name="dto">登録データ</param>
    public async Task CreateLogAsync(GourmetLog log)
    {
        await DbContext.GourmetLogs.AddAsync(log);
    }
}
