using LineWebHookAPI.Models.DB.Tables;
using LineWebHookAPI.Models.Exceptions;

namespace LineWebHookAPI.Models.DB.Repositories;

public abstract class MiddleWareRepositoryBase(LineWebHookContext dbContext) : Repository(dbContext)
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
/// ミドルウェア用リポジトリ
/// </summary>
public class MiddleWareRepository(LineWebHookContext dbContext) : MiddleWareRepositoryBase(dbContext)
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
