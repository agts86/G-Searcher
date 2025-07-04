using LineWebHookAPI.Models.DB.Tables;
using LineWebHookAPI.Models.Exceptions;

namespace LineWebHookAPI.Models.DB.Repositories;

public interface IMiddleWareRepository
{
    /// <summary>
    /// エラーログを作成する
    /// </summary>
    /// <param name="ex">例外</param>
    Task CreateErrorLogAsync(Exception ex);

    /// <summary>
    /// エラーログを作成する
    /// </summary>
    /// <param name="ex">例外</param>
    Task CreateErrorLogAsync(ResponseError error);

    /// <summary>
    /// データベースの変更を保存する
    /// </summary>
    Task SaveChangesAsync();
}

/// <summary>
/// ミドルウェア用リポジトリ
/// </summary>
public class MiddleWareRepository(LineWebHookContext dbContext) : IMiddleWareRepository
{
    /// <summary>
    /// EFCoreのコンテキスト
    /// </summary>
    private LineWebHookContext DbContext { get; } = dbContext;

    /// <summary>
    /// エラーログを作成する
    /// </summary>
    /// <param name="ex">例外</param>
    public async Task CreateErrorLogAsync(Exception ex)
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
    public async Task CreateErrorLogAsync(ResponseError error)
    {
        var errorLog = new ErrorLog
        {
            Id = Guid.NewGuid(),
            Contents = error.Message
        };
        await DbContext.ErrorLogs.AddAsync(errorLog);
    }

    /// <summary>
    /// データベースの変更を保存する
    /// </summary>
    public async Task SaveChangesAsync()
    {
        await DbContext.SaveChangesAsync();
    }
}
