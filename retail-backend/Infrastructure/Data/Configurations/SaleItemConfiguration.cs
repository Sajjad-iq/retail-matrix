using Domains.Sales.Entities;
using Domains.Sales.Enums;
using Domains.Shared.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for SaleItem entity
/// </summary>
public class SaleItemConfiguration : IEntityTypeConfiguration<SaleItem>
{
    public void Configure(EntityTypeBuilder<SaleItem> builder)
    {
        // Table name
        builder.ToTable("SaleItems");

        // Primary key
        builder.HasKey(i => i.Id);

        // Indexes
        builder.HasIndex(i => i.SaleId)
            .HasDatabaseName("IX_SaleItems_Sale");

        builder.HasIndex(i => i.ProductPackagingId)
            .HasDatabaseName("IX_SaleItems_ProductPackaging");

        // Properties configuration
        builder.Property(i => i.Id)
            .ValueGeneratedNever(); // Generated in domain

        builder.Property(i => i.SaleId)
            .IsRequired();

        builder.Property(i => i.ProductPackagingId)
            .IsRequired();

        builder.Property(i => i.ProductName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(i => i.Quantity)
            .IsRequired();

        // Complex properties (Value Objects)
        builder.ComplexProperty(i => i.UnitPrice, priceBuilder =>
        {
            priceBuilder.IsRequired();

            priceBuilder.Property(p => p.Amount)
                .HasColumnName("UnitPrice")
                .IsRequired()
                .HasPrecision(18, 2);

            priceBuilder.Property(p => p.Currency)
                .HasColumnName("UnitPriceCurrency")
                .IsRequired()
                .HasMaxLength(3)
                .HasDefaultValue("IQD");
        });

        builder.ComplexProperty(i => i.LineTotal, priceBuilder =>
        {
            priceBuilder.IsRequired();

            priceBuilder.Property(p => p.Amount)
                .HasColumnName("LineTotal")
                .IsRequired()
                .HasPrecision(18, 2);

            priceBuilder.Property(p => p.Currency)
                .HasColumnName("LineTotalCurrency")
                .IsRequired()
                .HasMaxLength(3)
                .HasDefaultValue("IQD");
        });

        // Discount is optional - store as nullable columns instead of complex property
        // EF Core doesn't support optional complex properties yet
        builder.Ignore(i => i.Discount);

        // Audit fields
        builder.Property(i => i.InsertDate)
            .IsRequired();

        builder.Property(i => i.UpdateDate)
            .IsRequired();
    }
}
