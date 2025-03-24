using LineWebHookAPI.Models.DB.Daos;

namespace LineWebHookAPI.Models.DB.Repositories;

/// <summary>
/// ミドルウェア用リポジトリ
/// </summary>
public class MiddleWareRepository(LineWebHookContext dbContext) : Repository(dbContext)
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
