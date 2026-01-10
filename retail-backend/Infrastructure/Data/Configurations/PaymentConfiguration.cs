using Domains.Entities;
using Domains.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for Payment entity
/// </summary>
public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        // Table name
        builder.ToTable("Payments");

        // Primary key
        builder.HasKey(p => p.Id);

        // Indexes
        builder.HasIndex(p => p.SaleId)
            .HasDatabaseName("IX_Payments_Sale");

        builder.HasIndex(p => p.PaymentMethod)
            .HasDatabaseName("IX_Payments_PaymentMethod");

        builder.HasIndex(p => p.Status)
            .HasDatabaseName("IX_Payments_Status");

        // Properties configuration
        builder.Property(p => p.Id)
            .ValueGeneratedNever(); // Generated in domain

        builder.Property(p => p.SaleId)
            .IsRequired();

        builder.Property(p => p.PaymentMethod)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(p => p.PaymentDate)
            .IsRequired();

        builder.Property(p => p.ReferenceNumber)
            .HasMaxLength(100);

        builder.Property(p => p.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasDefaultValue(PaymentStatus.Completed);

        // Complex properties (Value Objects)
        builder.ComplexProperty(p => p.Amount, priceBuilder =>
        {
            priceBuilder.IsRequired();

            priceBuilder.Property(price => price.Amount)
                .HasColumnName("Amount")
                .IsRequired()
                .HasPrecision(18, 2);

            priceBuilder.Property(price => price.Currency)
                .HasColumnName("Currency")
                .IsRequired()
                .HasMaxLength(3)
                .HasDefaultValue("IQD");
        });

        // Audit fields
        builder.Property(p => p.InsertDate)
            .IsRequired();

        builder.Property(p => p.UpdateDate)
            .IsRequired();
    }
}
