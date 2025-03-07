using G_Searcher.DB.Tables;
using G_Searcher.DB;
using G_Searcher.Models.Exceptions;

namespace G_Searcher.DB.Daos;

public class ErrorLogDao(MyContext dbContext) : Dao(dbContext)
{
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
