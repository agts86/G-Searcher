using LineWebHookAPI.Models.DB.Daos;

namespace LineWebHookAPI.Models.DB.Repositories;

/// <summary>
/// サーチコントローラー用リポジトリ
/// </summary>
public class ManagedRepository(LineWebHookContext dbContext) : Repository(dbContext)
{
    /// <summary>
    /// グルメログDAO
    /// </summary>
    public GourmetLogDao GourmetLogDao { get; } = new GourmetLogDao(dbContext);
}
