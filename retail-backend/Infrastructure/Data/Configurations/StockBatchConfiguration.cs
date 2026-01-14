using Domains.Stock.Entities;
using Domains.Products.Entities;
using Domains.Stock.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for StockBatch entity
/// </summary>
public class StockBatchConfiguration : IEntityTypeConfiguration<StockBatch>
{
    public void Configure(EntityTypeBuilder<StockBatch> builder)
    {
        // Table name
        builder.ToTable("StockBatches");

        // Primary key
        builder.HasKey(b => b.Id);

        // Indexes
        builder.HasIndex(b => b.ProductStockId)
            .HasDatabaseName("IX_StockBatches_Stock");

        builder.HasIndex(b => b.BatchNumber)
            .HasDatabaseName("IX_StockBatches_BatchNumber");

        builder.HasIndex(b => b.ExpirationDate)
            .HasDatabaseName("IX_StockBatches_Expiration");

        builder.HasIndex(b => b.Condition)
            .HasDatabaseName("IX_StockBatches_Condition");

        // Properties
        builder.Property(b => b.Id)
            .ValueGeneratedNever();

        builder.Property(b => b.ProductStockId)
            .IsRequired();

        builder.Property(b => b.BatchNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(b => b.Quantity)
            .IsRequired();

        builder.Property(b => b.RemainingQuantity)
            .IsRequired();

        builder.Property(b => b.Condition)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(b => b.ReceivedDate)
            .IsRequired();

        builder.Property(b => b.ExpirationDate)
            .IsRequired(false);

        builder.Property(b => b.CostPrice)
            .IsRequired()
            .HasPrecision(18, 2);

        // Relationships
        builder.HasOne(b => b.ProductStock)
            .WithMany(s => s.Batches)
            .HasForeignKey(b => b.ProductStockId)
            .OnDelete(DeleteBehavior.Cascade);

        // Soft delete
        builder.Property(b => b.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // Audit fields
        builder.Property(b => b.InsertDate)
            .IsRequired();

        builder.Property(b => b.UpdateDate)
            .IsRequired();

        builder.Property(b => b.DeletedAt)
            .IsRequired(false);
    }
}
