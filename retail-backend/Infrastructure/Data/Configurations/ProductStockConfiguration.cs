using Domains.Stock.Entities;
using Domains.Products.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for ProductStock entity
/// </summary>
public class ProductStockConfiguration : IEntityTypeConfiguration<ProductStock>
{
    public void Configure(EntityTypeBuilder<ProductStock> builder)
    {
        // Table name
        builder.ToTable("ProductStocks");

        // Primary key
        builder.HasKey(s => s.Id);

        // Indexes
        builder.HasIndex(s => s.ProductPackagingId)
            .HasDatabaseName("IX_ProductStocks_Packaging");

        builder.HasIndex(s => s.LocationId)
            .HasDatabaseName("IX_ProductStocks_Location");

        builder.HasIndex(s => s.OrganizationId)
            .HasDatabaseName("IX_ProductStocks_Organization");

        // Properties configuration
        builder.Property(s => s.Id)
            .ValueGeneratedNever(); // Generated in domain

        builder.Property(s => s.LastStocktakeDate)
            .IsRequired(false);

        builder.Property(s => s.OrganizationId)
            .IsRequired();

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
        builder.Ignore(s => s.GoodStock);
        builder.Ignore(s => s.DamagedStock);
        builder.Ignore(s => s.ExpiredStock);
        builder.Ignore(s => s.ReservedStock);
        builder.Ignore(s => s.TotalStock);
        builder.Ignore(s => s.AvailableStock);
        builder.Ignore(s => s.LastRestockDate);

        // Relationships
        builder.HasOne(s => s.Packaging)
            .WithMany()
            .HasForeignKey(s => s.ProductPackagingId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(s => s.Batches)
            .WithOne(b => b.ProductStock)
            .HasForeignKey(b => b.ProductStockId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
