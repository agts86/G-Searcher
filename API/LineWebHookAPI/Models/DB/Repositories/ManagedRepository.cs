using LineWebHookAPI.Models.DB.Daos;

namespace LineWebHookAPI.Models.DB.Repositories;

/// <summary>
/// サーチコントローラー用リポジトリ
/// </summary>
public class ManagedRepository(MyContext dbContext) : Repository(dbContext)
{
    /// <summary>
    /// グルメログDAO
    /// </summary>
    public GourmetLogDao GourmetLogDao { get; } = new GourmetLogDao(dbContext);

    /// <summary>
    /// エラーログDAO
    /// </summary>
    public ErrorLogDao ErrorLogDao { get; } = new ErrorLogDao(dbContext);
}
