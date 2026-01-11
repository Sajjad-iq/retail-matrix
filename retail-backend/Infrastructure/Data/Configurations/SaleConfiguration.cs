using Domains.Entities;
using Domains.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for Sale entity
/// </summary>
public class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        // Table name
        builder.ToTable("Sales");

        // Primary key
        builder.HasKey(s => s.Id);

        // Indexes
        builder.HasIndex(s => s.SaleNumber)
            .IsUnique()
            .HasDatabaseName("IX_Sales_SaleNumber");

        builder.HasIndex(s => s.OrganizationId)
            .HasDatabaseName("IX_Sales_Organization");

        builder.HasIndex(s => s.SaleDate)
            .HasDatabaseName("IX_Sales_SaleDate");

        builder.HasIndex(s => s.Status)
            .HasDatabaseName("IX_Sales_Status");

        builder.HasIndex(s => s.SalesPersonId)
            .HasDatabaseName("IX_Sales_SalesPerson");

        // Properties configuration
        builder.Property(s => s.Id)
            .ValueGeneratedNever(); // Generated in domain

        builder.Property(s => s.SaleNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.SaleDate)
            .IsRequired();

        builder.Property(s => s.SalesPersonId)
            .IsRequired();

        builder.Property(s => s.OrganizationId)
            .IsRequired();

        builder.Property(s => s.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasDefaultValue(SaleStatus.Draft);

        builder.Property(s => s.Notes)
            .HasMaxLength(2000);

        // Complex properties (Value Objects)
        builder.ComplexProperty(s => s.TotalDiscount, priceBuilder =>
        {
            priceBuilder.Property(p => p.Amount)
                .HasColumnName("TotalDiscount")
                .IsRequired()
                .HasPrecision(18, 2)
                .HasDefaultValue(0);

            priceBuilder.Property(p => p.Currency)
                .HasColumnName("TotalDiscountCurrency")
                .IsRequired()
                .HasMaxLength(3)
                .HasDefaultValue("IQD");
        });

        builder.ComplexProperty(s => s.GrandTotal, priceBuilder =>
        {
            priceBuilder.Property(p => p.Amount)
                .HasColumnName("GrandTotal")
                .IsRequired()
                .HasPrecision(18, 2);

            priceBuilder.Property(p => p.Currency)
                .HasColumnName("GrandTotalCurrency")
                .IsRequired()
                .HasMaxLength(3)
                .HasDefaultValue("IQD");
        });

        builder.ComplexProperty(s => s.AmountPaid, priceBuilder =>
        {
            priceBuilder.Property(p => p.Amount)
                .HasColumnName("AmountPaid")
                .IsRequired()
                .HasPrecision(18, 2)
                .HasDefaultValue(0);

            priceBuilder.Property(p => p.Currency)
                .HasColumnName("AmountPaidCurrency")
                .IsRequired()
                .HasMaxLength(3)
                .HasDefaultValue("IQD");
        });

        // Relationships
        builder.HasMany(s => s.Items)
            .WithOne()
            .HasForeignKey(i => i.SaleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.Payments)
            .WithOne()
            .HasForeignKey(p => p.SaleId)
            .OnDelete(DeleteBehavior.Cascade);

        // Soft delete
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
    }
}
