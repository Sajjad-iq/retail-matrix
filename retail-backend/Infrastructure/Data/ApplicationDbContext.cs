using Domains.Entities;
using Domains.Shared;
using Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

/// <summary>
/// Application database context for Entity Framework Core
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // DbSets
    public DbSet<User> Users => Set<User>();
    public DbSet<Organization> Organizations => Set<Organization>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply entity configurations
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new OrganizationConfiguration());

        // Global query filter for soft delete
        modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);
        modelBuilder.Entity<Organization>().HasQueryFilter(o => !o.IsDeleted);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Automatically set audit fields
        var entries = ChangeTracker.Entries<BaseEntity>();

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Property(nameof(BaseEntity.InsertDate)).CurrentValue = DateTime.UtcNow;
                    entry.Property(nameof(BaseEntity.UpdateDate)).CurrentValue = DateTime.UtcNow;
                    break;

                case EntityState.Modified:
                    entry.Property(nameof(BaseEntity.UpdateDate)).CurrentValue = DateTime.UtcNow;
                    break;

                case EntityState.Deleted:
                    // Implement soft delete
                    entry.State = EntityState.Modified;
                    entry.Property(nameof(BaseEntity.IsDeleted)).CurrentValue = true;
                    entry.Property(nameof(BaseEntity.DeletedAt)).CurrentValue = DateTimeOffset.UtcNow;
                    entry.Property(nameof(BaseEntity.UpdateDate)).CurrentValue = DateTime.UtcNow;
                    break;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
