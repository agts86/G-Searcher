using G_Searcher.Models.DB.Tables;
using Microsoft.EntityFrameworkCore;
using G_Searcher.Utilities;
using G_Searcher.Models.DB.SaveChanges;

namespace G_Searcher.Models.DB;

public class MyContext(DbContextOptions<MyContext> options) : DbContext(options)
{
    public virtual DbSet<ErrorLog> ErrorLogs {get;set;}

    public virtual DbSet<GourmetLog> GourmetLogs {get;set;}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ErrorLog>(entity =>
        {
            entity.HasKey(e => new {e.Id});
        });
        modelBuilder.Entity<GourmetLog>(entity =>
        {
            entity.HasKey(e => new {e.Id});
        });
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is not Meta meta) continue;

            var saveChanges = Polymorphism.CreatePolymorphismArray<ISaveChanges>();
                
            foreach (var saveChange in saveChanges)
            {
                if(entry.State != saveChange.EntityState) continue;
                saveChange.ChangeMeta(meta);
            }

        }
        return base.SaveChangesAsync(cancellationToken);
    }
}

