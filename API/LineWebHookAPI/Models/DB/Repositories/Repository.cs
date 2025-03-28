using LineWebHookAPI.Models.DB.Daos;
using LineWebHookAPI.Models.Exceptions;

namespace LineWebHookAPI.Models.DB.Repositories;

/// <summary>
/// リポジトリの基底クラス
/// </summary>
public abstract class Repository(LineWebHookContext dbContext)
{
    /// <summary>
    /// EFCoreのコンテキスト
    /// </summary>
    protected LineWebHookContext DbContext { get; } = dbContext;

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

     /// <summary>
    /// エラーログを作成する
    /// </summary>
    /// <param name="ex">例外</param>
    public async Task CreateErrorLogAsync(Exception ex)
    {
        var responseError = new ResponseError(ex.Message);
        await ErrorLogDao.CreateLogAsync(responseError);
    }
}
