using LineWebHookAPI.Models.DB.Tables;
using LineWebHookAPI.Models.Exceptions;

namespace LineWebHookAPI.Models.DB.Daos;

/// <summary>
/// エラーログDAO
/// </summary>
public class ErrorLogDao(MyContext dbContext) : Dao(dbContext)
{
    /// <summary>
    /// ログを作成する
    /// </summary>
    /// <param name="dto">エラー情報</param>
    public async Task CreateLogAsync(ResponseError error)
    {
        var errorLog = new ErrorLog
        {
            Id = Guid.NewGuid(),
            Contents = error.Message
        };
        await DbContext.ErrorLogs.AddAsync(errorLog);
    }
}
