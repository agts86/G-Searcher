using LineWebHookAPI.Models.DB.Tables;
using LineWebHookAPI.Models.DB.Daos;
using LineWebHookAPI.Models.Dto.Line.Hook.Messages;

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
}
