using LineWebHookAPI.Models.DB.Daos;

namespace LineWebHookAPI.Models.DB.Repositories;

/// <summary>
/// コントローラー基底クラス用リポジトリ
/// </summary>
public class BaseControllerRepository(LineWebHookContext dbContext) : Repository(dbContext)
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
