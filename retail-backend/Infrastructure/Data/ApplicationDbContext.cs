using Domains.Sales.Entities;
using Domains.Inventory.Entities;
using Domains.Products.Entities;
using Domains.Organizations.Entities;
using Domains.Users.Entities;
using Domains.Common.Currency.Entities;
using Domains.Shared.Base;
using Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using InventoryEntity = Domains.Inventory.Entities.Inventory;

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
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductPackaging> ProductPackagings => Set<ProductPackaging>();
    public DbSet<ProductStock> ProductStocks => Set<ProductStock>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<InventoryMovement> InventoryMovements => Set<InventoryMovement>();
    public DbSet<InventoryEntity> Inventories => Set<InventoryEntity>();
    public DbSet<InventoryOperation> InventoryOperations => Set<InventoryOperation>();
    public DbSet<InventoryOperationItem> InventoryOperationItems => Set<InventoryOperationItem>();
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<SaleItem> SaleItems => Set<SaleItem>();
    public DbSet<Currency> Currencies => Set<Currency>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply entity configurations
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new OrganizationConfiguration());
        modelBuilder.ApplyConfiguration(new ProductConfiguration());
        modelBuilder.ApplyConfiguration(new ProductPackagingConfiguration());
        modelBuilder.ApplyConfiguration(new ProductStockConfiguration());
        modelBuilder.ApplyConfiguration(new CategoryConfiguration());
        modelBuilder.ApplyConfiguration(new InventoryMovementConfiguration());
        modelBuilder.ApplyConfiguration(new InventoryConfiguration());
        modelBuilder.ApplyConfiguration(new InventoryOperationConfiguration());
        modelBuilder.ApplyConfiguration(new InventoryOperationItemConfiguration());
        modelBuilder.ApplyConfiguration(new SaleConfiguration());
        modelBuilder.ApplyConfiguration(new SaleItemConfiguration());
        modelBuilder.ApplyConfiguration(new CurrencyConfiguration());

        // Global query filter for soft delete
        modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);
        modelBuilder.Entity<Organization>().HasQueryFilter(o => !o.IsDeleted);
        modelBuilder.Entity<Product>().HasQueryFilter(p => !p.IsDeleted);
        modelBuilder.Entity<ProductPackaging>().HasQueryFilter(p => !p.IsDeleted);
        modelBuilder.Entity<ProductStock>().HasQueryFilter(s => !s.IsDeleted);
        modelBuilder.Entity<Category>().HasQueryFilter(c => !c.IsDeleted);
        modelBuilder.Entity<InventoryMovement>().HasQueryFilter(m => !m.IsDeleted);
        modelBuilder.Entity<InventoryEntity>().HasQueryFilter(i => !i.IsDeleted);
        modelBuilder.Entity<InventoryOperation>().HasQueryFilter(o => !o.IsDeleted);
        modelBuilder.Entity<InventoryOperationItem>().HasQueryFilter(i => !i.IsDeleted);
        modelBuilder.Entity<Sale>().HasQueryFilter(s => !s.IsDeleted);
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
