using G_Searcher.Models.DB.Tables;
using G_Searcher.Models.DB;

namespace G_Searcher.Models.DB.Daos;

public class GourmetLogDao(MyContext dbContext) : Dao(dbContext)
{
    public async Task CreateLogAsync(GourmetLog log)
    {
        await DbContext.GourmetLogs.AddAsync(log);
    }
}
