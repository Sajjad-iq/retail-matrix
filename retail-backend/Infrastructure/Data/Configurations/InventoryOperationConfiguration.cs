using Domains.Inventory.Entities;
using Domains.Inventory.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InventoryEntity = Domains.Inventory.Entities.Inventory;

namespace Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for InventoryOperation entity
/// </summary>
public class InventoryOperationConfiguration : IEntityTypeConfiguration<InventoryOperation>
{
    public void Configure(EntityTypeBuilder<InventoryOperation> builder)
    {
        // Table name
        builder.ToTable("InventoryOperations");

        // Primary key
        builder.HasKey(o => o.Id);

        // Indexes
        builder.HasIndex(o => o.OperationNumber)
            .IsUnique()
            .HasDatabaseName("IX_InventoryOperations_Number");

        builder.HasIndex(o => o.OrganizationId)
            .HasDatabaseName("IX_InventoryOperations_Organization");

        builder.HasIndex(o => o.OperationDate)
            .HasDatabaseName("IX_InventoryOperations_Date");

        builder.HasIndex(o => o.Status)
            .HasDatabaseName("IX_InventoryOperations_Status");

        // Properties
        builder.Property(o => o.Id)
            .ValueGeneratedNever();

        builder.Property(o => o.OperationType)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(o => o.OperationNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(o => o.OperationDate)
            .IsRequired();

        builder.Property(o => o.UserId)
            .IsRequired();

        builder.Property(o => o.OrganizationId)
            .IsRequired();

        builder.Property(o => o.SourceInventoryId)
            .IsRequired(false);

        builder.Property(o => o.DestinationInventoryId)
            .IsRequired(false);

        builder.Property(o => o.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(o => o.Notes)
            .HasMaxLength(1000);

        // Soft delete
        builder.Property(o => o.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // Audit fields
        builder.Property(o => o.InsertDate)
            .IsRequired();

        builder.Property(o => o.UpdateDate)
            .IsRequired();

        builder.Property(o => o.DeletedAt)
            .IsRequired(false);

        // Relationships
        builder.HasOne(o => o.SourceInventory)
            .WithMany()
            .HasForeignKey(o => o.SourceInventoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.DestinationInventory)
            .WithMany()
            .HasForeignKey(o => o.DestinationInventoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(o => o.Items)
            .WithOne(i => i.Operation)
            .HasForeignKey(i => i.InventoryOperationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
