using Domains.Installments.Entities;
using Domains.Installments.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for InstallmentPayment entity
/// </summary>
public class InstallmentPaymentConfiguration : IEntityTypeConfiguration<InstallmentPayment>
{
    public void Configure(EntityTypeBuilder<InstallmentPayment> builder)
    {
        // Table name
        builder.ToTable("InstallmentPayments");

        // Primary key
        builder.HasKey(p => p.Id);

        // Indexes
        builder.HasIndex(p => p.InstallmentPlanId)
            .HasDatabaseName("IX_InstallmentPayments_InstallmentPlan");

        builder.HasIndex(p => p.DueDate)
            .HasDatabaseName("IX_InstallmentPayments_DueDate");

        builder.HasIndex(p => p.Status)
            .HasDatabaseName("IX_InstallmentPayments_Status");

        builder.HasIndex(p => p.PaymentDate)
            .HasDatabaseName("IX_InstallmentPayments_PaymentDate");

        builder.HasIndex(p => p.ReceivedByUserId)
            .HasDatabaseName("IX_InstallmentPayments_ReceivedBy");

        // Properties configuration
        builder.Property(p => p.Id)
            .ValueGeneratedNever();

        builder.Property(p => p.InstallmentPlanId)
            .IsRequired();

        builder.Property(p => p.DueDate)
            .IsRequired();

        builder.Property(p => p.InstallmentNumber)
            .IsRequired();

        builder.Property(p => p.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(InstallmentPaymentStatus.Pending);

        builder.Property(p => p.PaymentDate)
            .IsRequired(false);

        builder.Property(p => p.Reference)
            .HasMaxLength(100);

        builder.Property(p => p.Notes)
            .HasMaxLength(500);

        builder.Property(p => p.ReceivedByUserId)
            .IsRequired(false);

        // Complex properties (Value Objects)
        builder.ComplexProperty(p => p.Amount, priceBuilder =>
        {
            priceBuilder.Property(pr => pr.Amount)
                .HasColumnName("Amount")
                .IsRequired()
                .HasPrecision(18, 2);

            priceBuilder.Property(pr => pr.Currency)
                .HasColumnName("AmountCurrency")
                .IsRequired()
                .HasMaxLength(3)
                .HasDefaultValue("IQD");
        });

        builder.ComplexProperty(p => p.PaidAmount, priceBuilder =>
        {
            priceBuilder.Property(pr => pr.Amount)
                .HasColumnName("PaidAmount")
                .IsRequired()
                .HasPrecision(18, 2)
                .HasDefaultValue(0);

            priceBuilder.Property(pr => pr.Currency)
                .HasColumnName("PaidAmountCurrency")
                .IsRequired()
                .HasMaxLength(3)
                .HasDefaultValue("IQD");
        });

        // Soft delete
        builder.Property(p => p.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // Audit fields
        builder.Property(p => p.InsertDate)
            .IsRequired();

        builder.Property(p => p.UpdateDate)
            .IsRequired();

        builder.Property(p => p.DeletedAt)
            .IsRequired(false);
    }
}
