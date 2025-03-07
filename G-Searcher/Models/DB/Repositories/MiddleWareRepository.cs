using G_Searcher.Models.DB.Daos;

namespace G_Searcher.Models.DB.Repositories;

public class MiddleWareRepository(MyContext dbContext)
{
    private MyContext DbContext { get; } = dbContext;
    public ErrorLogDao ErrorLogDao { get; } = new ErrorLogDao(dbContext);

    public async Task SaveChangesAsync()
    {
        await DbContext.SaveChangesAsync();
    }
}
