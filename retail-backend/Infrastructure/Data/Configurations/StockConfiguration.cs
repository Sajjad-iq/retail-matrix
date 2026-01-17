using Domains.Stocks.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for Stock entity
/// </summary>
public class StockConfiguration : IEntityTypeConfiguration<Stock>
{
    public void Configure(EntityTypeBuilder<Stock> builder)
    {
        // Table name - keep ProductStocks for migration compatibility
        builder.ToTable("ProductStocks");

        // Primary key
        builder.HasKey(s => s.Id);

        // Indexes
        builder.HasIndex(s => s.ProductPackagingId)
            .HasDatabaseName("IX_ProductStocks_Packaging");

        builder.HasIndex(s => s.OrganizationId)
            .HasDatabaseName("IX_ProductStocks_Organization");

        builder.HasIndex(s => s.InventoryId)
            .HasDatabaseName("IX_ProductStocks_Location");

        // Unique constraint: one stock record per packaging per organization per inventory
        builder.HasIndex(s => new { s.ProductPackagingId, s.OrganizationId, s.InventoryId })
            .IsUnique()
            .HasDatabaseName("IX_ProductStocks_Packaging_Organization_Location");

        // Properties configuration
        builder.Property(s => s.Id)
            .ValueGeneratedNever(); // Generated in domain

        builder.Property(s => s.ProductPackagingId)
            .IsRequired();

        builder.Property(s => s.OrganizationId)
            .IsRequired();

        builder.Property(s => s.InventoryId)
            .IsRequired()
            .HasColumnName("LocationId"); // Keep column name for migration compatibility

        builder.Property(s => s.LastStocktakeDate)
            .IsRequired(false);

        builder.Property(s => s.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // Audit fields
        builder.Property(s => s.InsertDate)
            .IsRequired();

        builder.Property(s => s.UpdateDate)
            .IsRequired();

        builder.Property(s => s.DeletedAt)
            .IsRequired(false);

        // Computed properties (not persisted - calculated from batches)
        builder.Ignore(s => s.TotalQuantity);
        builder.Ignore(s => s.TotalReservedQuantity);
        builder.Ignore(s => s.TotalAvailableQuantity);

        // Batches navigation - configured in StockBatchConfiguration
        builder.HasMany(s => s.Batches)
            .WithOne(b => b.Stock)
            .HasForeignKey(b => b.StockId);
    }
}
