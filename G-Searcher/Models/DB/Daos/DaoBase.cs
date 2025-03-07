namespace G_Searcher.Models.DB.Daos;

public class Dao(MyContext dbContext)
{
    public MyContext DbContext { get; } = dbContext;
}
