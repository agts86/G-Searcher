using G_Searcher.DB.Tables;
using Microsoft.EntityFrameworkCore;

namespace G_Searcher.DB.SaveChanges;

public class SaveChangesAdd : ISaveChanges
{
    public EntityState EntityState { get; set; } = EntityState.Added;
    public void ChangeMeta(Meta meta)
    {
        meta.CreatedAt = DateTime.Now;
        meta.UpdatedAt = DateTime.Now;
    }
}
