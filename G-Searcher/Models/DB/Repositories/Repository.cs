namespace G_Searcher.Models.DB.Repositories;

public abstract class Repository(MyContext dbContext)
{
    protected MyContext DbContext { get; } = dbContext;
}
