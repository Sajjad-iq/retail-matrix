using Domains.Inventory.Entities;
using Domains.Products.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for InventoryOperationItem entity
/// </summary>
public class InventoryOperationItemConfiguration : IEntityTypeConfiguration<InventoryOperationItem>
{
    public void Configure(EntityTypeBuilder<InventoryOperationItem> builder)
    {
        // Table name
        builder.ToTable("InventoryOperationItems");

        // Primary key
        builder.HasKey(i => i.Id);

        // Indexes
        builder.HasIndex(i => i.InventoryOperationId)
            .HasDatabaseName("IX_InventoryOperationItems_Operation");

        builder.HasIndex(i => i.ProductPackagingId)
            .HasDatabaseName("IX_InventoryOperationItems_Packaging");

        // Properties
        builder.Property(i => i.Id)
            .ValueGeneratedNever();

        builder.Property(i => i.InventoryOperationId)
            .IsRequired();

        builder.Property(i => i.ProductPackagingId)
            .IsRequired();

        builder.Property(i => i.ProductName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(i => i.Barcode)
            .HasMaxLength(50);

        builder.Property(i => i.Quantity)
            .IsRequired();

        builder.Property(i => i.BalanceAfter)
            .IsRequired();

        // Price value object
        builder.OwnsOne(i => i.UnitPrice, price =>
        {
            price.Property(p => p.Amount)
                .HasColumnName("UnitPriceAmount")
                .HasPrecision(18, 2)
                .IsRequired();

            price.Property(p => p.Currency)
                .HasColumnName("UnitPriceCurrency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.Property(i => i.Notes)
            .HasMaxLength(500);

        // Soft delete
        builder.Property(i => i.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // Audit fields
        builder.Property(i => i.InsertDate)
            .IsRequired();

        builder.Property(i => i.UpdateDate)
            .IsRequired();

        builder.Property(i => i.DeletedAt)
            .IsRequired(false);

        // Relationships
        builder.HasOne(i => i.Operation)
            .WithMany(o => o.Items)
            .HasForeignKey(i => i.InventoryOperationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Foreign key to ProductPackaging (no navigation property - cross-domain reference)
        builder.Property(i => i.ProductPackagingId)
            .IsRequired();
    }
}
