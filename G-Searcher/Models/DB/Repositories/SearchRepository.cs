using G_Searcher.Models.DB.Tables;
using G_Searcher.Models.Dto.Search;
using G_Searcher.Models.DB.Daos;

namespace G_Searcher.Models.DB.Repositories;

public class SearchRepository(MyContext dbContext) : Repository(dbContext)
{
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
