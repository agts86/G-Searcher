using LineWebHookAPI.Models.DB.Repositories;
using LineWebHookAPI.Models.DB;
using LineWebHookAPI.Models.DB.Tables;

namespace LineWebHookAPI.Models.BL.Managed;

/// <summary>
/// 管理用コントローラーのビジネスロジック
/// </summary>
public class ManagedBL(MyContext dbContext)
{
    /// <summary>
    /// リポジトリ
    /// </summary>
    public HotPepperRepository HotPepperRepository { get; protected set; } = new HotPepperRepository(dbContext);

    public async Task<GourmetLog[]> GetLogAsync()
    {
        return await HotPepperRepository.GourmetLogDao.FetchLogAsync();
    }
}
