using Domains.Inventory.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InventoryEntity = Domains.Inventory.Entities.Inventory;

namespace Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for Inventory entity (storage locations)
/// </summary>
public class InventoryConfiguration : IEntityTypeConfiguration<InventoryEntity>
{
    public void Configure(EntityTypeBuilder<InventoryEntity> builder)
    {
        // Table name - keep same table for migration compatibility
        builder.ToTable("Locations");

        // Primary key
        builder.HasKey(i => i.Id);

        // Indexes
        builder.HasIndex(i => i.OrganizationId)
            .HasDatabaseName("IX_Locations_Organization");

        builder.HasIndex(i => new { i.Code, i.OrganizationId })
            .IsUnique()
            .HasDatabaseName("IX_Locations_Code_Organization");

        builder.HasIndex(i => i.Type)
            .HasDatabaseName("IX_Locations_Type");

        builder.HasIndex(i => i.ParentId)
            .HasDatabaseName("IX_Locations_Parent");

        // Properties
        builder.Property(i => i.Id)
            .ValueGeneratedNever();

        builder.Property(i => i.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(i => i.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(i => i.Type)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(i => i.OrganizationId)
            .IsRequired();

        builder.Property(i => i.ParentId)
            .IsRequired(false);

        builder.Property(i => i.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Self-referencing relationship (hierarchical)
        builder.HasOne(i => i.Parent)
            .WithMany(i => i.Children)
            .HasForeignKey(i => i.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

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
    }
}
