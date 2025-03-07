using G_Searcher.DB.Tables;
using G_Searcher.DB;

namespace G_Searcher.DB.Daos;

public class GourmetLogDao(MyContext dbContext) : Dao(dbContext)
{
    public async Task CreateLogAsync(GourmetLog log)
    {
        await DbContext.GourmetLogs.AddAsync(log);
    }
}
