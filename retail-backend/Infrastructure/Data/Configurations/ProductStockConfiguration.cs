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

        builder.HasIndex(s => s.OrganizationId)
            .HasDatabaseName("IX_ProductStocks_Organization");

        // Unique constraint: one stock record per packaging per organization
        builder.HasIndex(s => new { s.ProductPackagingId, s.OrganizationId })
            .IsUnique()
            .HasDatabaseName("IX_ProductStocks_Packaging_Organization");

        // Properties configuration
        builder.Property(s => s.Id)
            .ValueGeneratedNever(); // Generated in domain

        builder.Property(s => s.ProductPackagingId)
            .IsRequired();

        builder.Property(s => s.OrganizationId)
            .IsRequired();

        builder.Property(s => s.Quantity)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(s => s.ReservedQuantity)
            .IsRequired()
            .HasDefaultValue(0);

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

        // Computed property (not persisted)
        builder.Ignore(s => s.AvailableQuantity);

        // Relationships
        builder.HasOne(s => s.Packaging)
            .WithMany()
            .HasForeignKey(s => s.ProductPackagingId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
