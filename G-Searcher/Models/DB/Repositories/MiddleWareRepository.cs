using G_Searcher.DB.Daos;

namespace G_Searcher.DB.Repositories;

public class MiddleWareRepository(MyContext dbContext)
{
    private MyContext DbContext { get; } = dbContext;
    public ErrorLogDao ErrorLogDao { get; } = new ErrorLogDao(dbContext);

    public async Task SaveChangesAsync()
    {
        await DbContext.SaveChangesAsync();
    }
}
