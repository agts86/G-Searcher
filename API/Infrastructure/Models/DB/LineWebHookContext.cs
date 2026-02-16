using Application.Models.DB.Tables;
using Microsoft.EntityFrameworkCore;
using Application.Utilities;
using Infrastructure.Models.DB.SaveChanges;

namespace Infrastructure.Models.DB;

public class LineWebHookContext(DbContextOptions<LineWebHookContext> options) : DbContext(options)
{
    public virtual DbSet<ErrorLog> ErrorLogs {get;set;}

    public virtual DbSet<GourmetLocationLog> GourmetLocationLogs {get;set;}
    
    public virtual DbSet<GourmetWordLog> GourmetWordLogs { get; set; }

    public virtual DbSet<JobLog> JobLogs { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

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
        modelBuilder.Entity<JobLog>(entity =>
        {
            entity.HasKey(e => e.Id);
        });
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.TokenHash).IsUnique();
            entity.HasIndex(e => e.ExpiresAt);
        });
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var saveChanges = Polymorphism.CreatePolymorphismArray<ISaveChanges>(typeof(LineWebHookContext).Assembly);
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
