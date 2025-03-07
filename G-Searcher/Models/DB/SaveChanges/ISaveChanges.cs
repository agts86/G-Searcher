using G_Searcher.Models.DB.Tables;
using Microsoft.EntityFrameworkCore;

namespace G_Searcher.Models.DB.SaveChanges;

public interface ISaveChanges
{
    public EntityState EntityState { get; set; }

    public void ChangeMeta(Meta meta);
}
