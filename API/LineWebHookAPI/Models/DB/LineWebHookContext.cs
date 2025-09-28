using LineWebHookAPI.Models.DB.Tables;
using Microsoft.EntityFrameworkCore;
using LineWebHookAPI.Utilities;
using LineWebHookAPI.Models.DB.SaveChanges;

namespace LineWebHookAPI.Models.DB;

public class LineWebHookContext(DbContextOptions<LineWebHookContext> options) : DbContext(options)
{
    public virtual DbSet<ErrorLog> ErrorLogs {get;set;}

    public virtual DbSet<GourmetLocationLog> GourmetLocationLogs {get;set;}
    
    public virtual DbSet<GourmetWordLog> GourmetWordLogs {get;set;}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ErrorLog>(entity =>
        {
            entity.HasKey(e => e.Id);
        });
        modelBuilder.Entity<GourmetLocationLog>(entity =>
        {
            entity.HasKey(e => e.Id);
        });
        modelBuilder.Entity<GourmetWordLog>(entity =>
        {
            entity.HasKey(e => e.Id);
        });
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var saveChanges = Polymorphism.CreatePolymorphismArray<ISaveChanges>();
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is not Meta meta) continue;
            foreach (var saveChange in saveChanges)
            {
                if (entry.State != saveChange.EntityState) continue;
                saveChange.ChangeMeta(meta);
            }

        }
        return base.SaveChangesAsync(cancellationToken);
    }
}

