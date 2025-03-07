namespace G_Searcher.Models.DB.Daos;

public abstract class Dao(MyContext dbContext)
{
    public MyContext DbContext { get; } = dbContext;
}
