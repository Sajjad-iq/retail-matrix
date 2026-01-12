using Domains.Common.Currency.Entities;
using Domains.Common.Currency.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class CurrencyConfiguration : IEntityTypeConfiguration<Currency>
{
    public void Configure(EntityTypeBuilder<Currency> builder)
    {
        builder.ToTable("Currencies");

        // Primary Key
        builder.HasKey(c => c.Id);

        // Indexes
        builder.HasIndex(c => new { c.Code, c.OrganizationId })
            .IsUnique()
            .HasDatabaseName("IX_Currencies_Code_Organization");

        builder.HasIndex(c => new { c.Name, c.OrganizationId })
            .IsUnique()
            .HasDatabaseName("IX_Currencies_Name_Organization");

        builder.HasIndex(c => c.OrganizationId)
            .HasDatabaseName("IX_Currencies_Organization");

        builder.HasIndex(c => new { c.IsBaseCurrency, c.OrganizationId })
            .HasDatabaseName("IX_Currencies_BaseCurrency_Organization");

        // Properties
        builder.Property(c => c.Code)
            .IsRequired()
            .HasMaxLength(3);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Symbol)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(c => c.ExchangeRate)
            .IsRequired()
            .HasPrecision(18, 6);

        builder.Property(c => c.IsBaseCurrency)
            .IsRequired();

        builder.Property(c => c.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(c => c.OrganizationId)
            .IsRequired();

        // Soft Delete Query Filter
        builder.HasQueryFilter(c => !c.IsDeleted);
    }
}
