using G_Searcher.DB.Tables;
using G_Searcher.Models.Dto.Search;
using G_Searcher.DB.Daos;

namespace G_Searcher.DB.Repositories;

public class SearchRepository(MyContext dbContext)
{
    private MyContext DbContext { get; } = dbContext;
    public GourmetLogDao GourmetLogDao { get; } = new GourmetLogDao(dbContext);

    public async Task CreateLogAsync(GourmetGettingDto dto)
    {
        var log = new GourmetLog
        {
            Id = Guid.NewGuid(),
            Lat = dto.Lat,
            Lng = dto.Lng
        };
        await GourmetLogDao.CreateLogAsync(log);
    }

    public async Task SaveChangesAsync()
    {
        await DbContext.SaveChangesAsync();
    }
}
