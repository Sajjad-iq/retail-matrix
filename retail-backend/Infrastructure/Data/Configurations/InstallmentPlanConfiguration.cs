using Domains.Installments.Entities;
using Domains.Installments.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for InstallmentPlan entity
/// </summary>
public class InstallmentPlanConfiguration : IEntityTypeConfiguration<InstallmentPlan>
{
    public void Configure(EntityTypeBuilder<InstallmentPlan> builder)
    {
        // Table name
        builder.ToTable("InstallmentPlans");

        // Primary key
        builder.HasKey(p => p.Id);

        // Indexes
        builder.HasIndex(p => p.PlanNumber)
            .IsUnique()
            .HasDatabaseName("IX_InstallmentPlans_PlanNumber");

        builder.HasIndex(p => p.OrganizationId)
            .HasDatabaseName("IX_InstallmentPlans_Organization");

        builder.HasIndex(p => p.SaleId)
            .HasDatabaseName("IX_InstallmentPlans_Sale");

        builder.HasIndex(p => p.Status)
            .HasDatabaseName("IX_InstallmentPlans_Status");

        builder.HasIndex(p => p.CustomerId)
            .HasDatabaseName("IX_InstallmentPlans_Customer");

        // Properties configuration
        builder.Property(p => p.Id)
            .ValueGeneratedNever();

        builder.Property(p => p.PlanNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.OrganizationId)
            .IsRequired();

        builder.Property(p => p.SaleId)
            .IsRequired();

        builder.Property(p => p.CustomerId)
            .IsRequired();

        builder.Property(p => p.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(InstallmentPlanStatus.Draft);

        builder.Property(p => p.CreatedByUserId)
            .IsRequired();

        builder.Property(p => p.ApprovedByUserId)
            .IsRequired(false);

        builder.Property(p => p.ApprovedAt)
            .IsRequired(false);

        builder.Property(p => p.Notes)
            .HasMaxLength(2000);

        // Complex properties (Value Objects)
        builder.ComplexProperty(p => p.TotalAmount, priceBuilder =>
        {
            priceBuilder.Property(pr => pr.Amount)
                .HasColumnName("TotalAmount")
                .IsRequired()
                .HasPrecision(18, 2);

            priceBuilder.Property(pr => pr.Currency)
                .HasColumnName("TotalAmountCurrency")
                .IsRequired()
                .HasMaxLength(3)
                .HasDefaultValue("IQD");
        });

        builder.ComplexProperty(p => p.DownPayment, priceBuilder =>
        {
            priceBuilder.Property(pr => pr.Amount)
                .HasColumnName("DownPayment")
                .IsRequired()
                .HasPrecision(18, 2)
                .HasDefaultValue(0);

            priceBuilder.Property(pr => pr.Currency)
                .HasColumnName("DownPaymentCurrency")
                .IsRequired()
                .HasMaxLength(3)
                .HasDefaultValue("IQD");
        });

        builder.ComplexProperty(p => p.InterestAmount, priceBuilder =>
        {
            priceBuilder.Property(pr => pr.Amount)
                .HasColumnName("InterestAmount")
                .IsRequired()
                .HasPrecision(18, 2)
                .HasDefaultValue(0);

            priceBuilder.Property(pr => pr.Currency)
                .HasColumnName("InterestAmountCurrency")
                .IsRequired()
                .HasMaxLength(3)
                .HasDefaultValue("IQD");
        });

        // Relationships
        builder.HasMany(p => p.Payments)
            .WithOne(pay => pay.InstallmentPlan)
            .HasForeignKey(pay => pay.InstallmentPlanId)
            .OnDelete(DeleteBehavior.Cascade);

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
