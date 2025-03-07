using G_Searcher.DB.Tables;
using Microsoft.EntityFrameworkCore;

namespace G_Searcher.DB.SaveChanges;

public class SaveChangesModify: ISaveChanges
{
    public EntityState EntityState { get; set; } = EntityState.Modified;
    public void ChangeMeta(Meta meta)
    {
        meta.UpdatedAt = DateTime.Now;
    }
}
