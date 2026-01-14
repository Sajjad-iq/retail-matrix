using Domains.Stock.Entities;
using Domains.Products.Entities;
using Domains.Stock.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for StockMovement entity
/// </summary>
public class StockMovementConfiguration : IEntityTypeConfiguration<StockMovement>
{
    public void Configure(EntityTypeBuilder<StockMovement> builder)
    {
        // Table name
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

        builder.HasIndex(m => m.BatchNumber)
            .HasDatabaseName("IX_StockMovements_Batch");

        builder.HasIndex(m => m.OrganizationId)
            .HasDatabaseName("IX_StockMovements_Organization");

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

        builder.Property(m => m.BatchNumber)
            .HasMaxLength(50);

        builder.Property(m => m.ExpirationDate)
            .IsRequired(false);

        // Relationships
        builder.HasOne(m => m.Packaging)
            .WithMany()
            .HasForeignKey(m => m.ProductPackagingId)
            .OnDelete(DeleteBehavior.Restrict);

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
