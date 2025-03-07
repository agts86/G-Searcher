namespace G_Searcher.Models.DB.Daos;

public abstract class Dao(MyContext dbContext)
{
    protected MyContext DbContext { get; } = dbContext;
}
