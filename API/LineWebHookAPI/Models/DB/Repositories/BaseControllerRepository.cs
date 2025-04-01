using LineWebHookAPI.Models.DB.Tables;
using LineWebHookAPI.Models.Exceptions;

namespace LineWebHookAPI.Models.DB.Repositories;

/// <summary>
/// コントローラー基底クラス用リポジトリ
/// </summary>
public abstract class BaseControllerRepositoryBase(LineWebHookContext dbContext) : Repository(dbContext)
{
    /// <summary>
    /// エラーログを作成する
    /// </summary>
    /// <param name="ex">例外</param>
    public abstract Task CreateErrorLogAsync(Exception ex);

    /// <summary>
    /// エラーログを作成する
    /// </summary>
    /// <param name="ex">例外</param>
    public abstract Task CreateErrorLogAsync(ResponseError error);
}

/// <summary>
/// コントローラー基底クラス用リポジトリ
/// </summary>
public class BaseControllerRepository(LineWebHookContext dbContext) : BaseControllerRepositoryBase(dbContext)
{
    /// <summary>
    /// エラーログを作成する
    /// </summary>
    /// <param name="ex">例外</param>
    public override async Task CreateErrorLogAsync(Exception ex)
    {
        var responseError = new ResponseError(ex.Message);
        var errorLog = new ErrorLog
        {
            Id = Guid.NewGuid(),
            Contents = responseError.Message
        };
        await DbContext.ErrorLogs.AddAsync(errorLog);
    }

    /// <summary>
    /// エラーログを作成する
    /// </summary>
    /// <param name="ex">例外</param>
    public override async Task CreateErrorLogAsync(ResponseError error)
    {
        var errorLog = new ErrorLog
        {
            Id = Guid.NewGuid(),
            Contents = error.Message
        };
        await DbContext.ErrorLogs.AddAsync(errorLog);
    }
}
