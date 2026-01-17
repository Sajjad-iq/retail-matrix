using Domains.Stocks.Entities;
using Domains.Stocks.Enums;
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
        builder.ToTable("StockBatches");

        // Primary key
        builder.HasKey(b => b.Id);

        // Indexes
        builder.HasIndex(b => b.StockId)
            .HasDatabaseName("IX_StockBatches_Stock");

        builder.HasIndex(b => b.BatchNumber)
            .HasDatabaseName("IX_StockBatches_BatchNumber");

        builder.HasIndex(b => b.ExpiryDate)
            .HasDatabaseName("IX_StockBatches_ExpiryDate");

        builder.HasIndex(b => new { b.StockId, b.BatchNumber })
            .IsUnique()
            .HasDatabaseName("IX_StockBatches_Stock_BatchNumber");

        // Properties
        builder.Property(b => b.Id)
            .ValueGeneratedNever();

        builder.Property(b => b.StockId)
            .IsRequired();

        builder.Property(b => b.BatchNumber)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(b => b.Quantity)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(b => b.ReservedQuantity)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(b => b.ExpiryDate)
            .IsRequired(false);

        builder.Property(b => b.Condition)
            .IsRequired()
            .HasDefaultValue(StockCondition.New)
            .HasConversion<string>()
            .HasMaxLength(20);

        // CostPrice as owned value object
        builder.OwnsOne(b => b.CostPrice, price =>
        {
            price.Property(p => p.Amount)
                .HasColumnName("CostPriceAmount")
                .HasPrecision(18, 4);
            price.Property(p => p.Currency)
                .HasColumnName("CostPriceCurrency")
                .HasMaxLength(3);
        });

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

        // Computed property (not persisted)
        builder.Ignore(b => b.AvailableQuantity);

        // Relationship to Stock
        builder.HasOne(b => b.Stock)
            .WithMany(s => s.Batches)
            .HasForeignKey(b => b.StockId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
