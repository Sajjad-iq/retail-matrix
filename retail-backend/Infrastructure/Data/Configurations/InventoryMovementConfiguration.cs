using Domains.Inventory.Entities;
using Domains.Products.Entities;
using Domains.Inventory.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InventoryEntity = Domains.Inventory.Entities.Inventory;

namespace Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for InventoryMovement entity
/// </summary>
public class InventoryMovementConfiguration : IEntityTypeConfiguration<InventoryMovement>
{
    public void Configure(EntityTypeBuilder<InventoryMovement> builder)
    {
        // Table name - keep same table for migration compatibility
        builder.ToTable("StockMovements");

        // Primary key
        builder.HasKey(m => m.Id);

        // Indexes
        builder.HasIndex(m => m.ProductPackagingId)
            .HasDatabaseName("IX_StockMovements_Packaging");

        builder.HasIndex(m => m.MovementDate)
            .HasDatabaseName("IX_StockMovements_Date");

        builder.HasIndex(m => m.Type)
            .HasDatabaseName("IX_StockMovements_Type");

        builder.HasIndex(m => m.ReferenceNumber)
            .HasDatabaseName("IX_StockMovements_Reference");

        builder.HasIndex(m => m.OrganizationId)
            .HasDatabaseName("IX_StockMovements_Organization");

        builder.HasIndex(m => m.InventoryId)
            .HasDatabaseName("IX_StockMovements_Location");

        // Properties
        builder.Property(m => m.Id)
            .ValueGeneratedNever();

        builder.Property(m => m.ProductPackagingId)
            .IsRequired();

        builder.Property(m => m.Quantity)
            .IsRequired();

        builder.Property(m => m.BalanceAfter)
            .IsRequired();

        builder.Property(m => m.Type)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(m => m.Reason)
            .HasMaxLength(500);

        builder.Property(m => m.ReferenceNumber)
            .HasMaxLength(50);

        builder.Property(m => m.MovementDate)
            .IsRequired();

        builder.Property(m => m.UserId)
            .IsRequired();

        builder.Property(m => m.OrganizationId)
            .IsRequired();

        builder.Property(m => m.InventoryId)
            .IsRequired(false)
            .HasColumnName("LocationId"); // Keep column name for migration compatibility

        builder.Property(m => m.InventoryOperationItemId)
            .IsRequired(false);

        // Relationships
        builder.HasOne(m => m.Packaging)
            .WithMany()
            .HasForeignKey(m => m.ProductPackagingId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.Inventory)
            .WithMany()
            .HasForeignKey(m => m.InventoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.OperationItem)
            .WithMany()
            .HasForeignKey(m => m.InventoryOperationItemId)
            .OnDelete(DeleteBehavior.SetNull);

        // Soft delete
        builder.Property(m => m.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // Audit fields
        builder.Property(m => m.InsertDate)
            .IsRequired();

        builder.Property(m => m.UpdateDate)
            .IsRequired();

        builder.Property(m => m.DeletedAt)
            .IsRequired(false);
    }
}
