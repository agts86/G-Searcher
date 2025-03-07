using G_Searcher.DB;

namespace G_Searcher.DB.Daos;

public class Dao(MyContext dbContext)
{
    public MyContext DbContext { get; } = dbContext;
}
