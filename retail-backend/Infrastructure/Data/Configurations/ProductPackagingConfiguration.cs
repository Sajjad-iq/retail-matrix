using Domains.Products.Entities;
using Domains.Products.Enums;
using Domains.Shared.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for ProductPackaging entity
/// </summary>
public class ProductPackagingConfiguration : IEntityTypeConfiguration<ProductPackaging>
{
    public void Configure(EntityTypeBuilder<ProductPackaging> builder)
    {
        // Table name
        builder.ToTable("ProductPackagings");

        // Primary key
        builder.HasKey(p => p.Id);

        // Indexes
        builder.HasIndex(p => p.Barcode)
            .IsUnique()
            .HasDatabaseName("IX_ProductPackagings_Barcode");

        builder.HasIndex(p => p.ProductId)
            .HasDatabaseName("IX_ProductPackagings_Product");

        builder.HasIndex(p => p.OrganizationId)
            .HasDatabaseName("IX_ProductPackagings_Organization");

        builder.HasIndex(p => p.IsDefault)
            .HasDatabaseName("IX_ProductPackagings_IsDefault");

        // Properties configuration
        builder.Property(p => p.Id)
            .ValueGeneratedNever(); // Generated in domain

        builder.Property(p => p.Barcode)
            .HasMaxLength(13);

        builder.Property(p => p.UnitsPerPackage)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(p => p.UnitOfMeasure)
            .IsRequired()
            .HasConversion<string>();

        // Configure Price value objects using ComplexProperty
        builder.ComplexProperty(p => p.CostPrice, priceBuilder =>
        {
            priceBuilder.Property(price => price.Amount)
                .HasColumnName("CostPrice")
                .IsRequired()
                .HasPrecision(18, 2);

            priceBuilder.Property(price => price.Currency)
                .HasColumnName("CostCurrency")
                .IsRequired()
                .HasMaxLength(3)
                .HasDefaultValue("IQD");
        });

        builder.ComplexProperty(p => p.SellingPrice, priceBuilder =>
        {
            priceBuilder.Property(price => price.Amount)
                .HasColumnName("SellingPrice")
                .IsRequired()
                .HasPrecision(18, 2);

            priceBuilder.Property(price => price.Currency)
                .HasColumnName("SellingCurrency")
                .IsRequired()
                .HasMaxLength(3)
                .HasDefaultValue("IQD");
        });

        builder.Property(p => p.ReorderLevel)
            .IsRequired()
            .HasDefaultValue(10);

        builder.Property(p => p.IsDefault)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(p => p.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(p => p.ImageUrl)
            .HasMaxLength(500);

        builder.Property(p => p.Dimensions)
            .HasMaxLength(100);

        // Configure Weight value object using ComplexProperty
        builder.ComplexProperty(p => p.Weight, weightBuilder =>
        {
            weightBuilder.IsRequired();

            weightBuilder.Property(weight => weight.Value)
                .HasColumnName("Weight")
                .HasPrecision(18, 3);

            weightBuilder.Property(weight => weight.Unit)
                .HasColumnName("WeightUnit")
                .HasConversion<string>();
        });

        builder.Property(p => p.Color)
            .HasMaxLength(50);

        builder.Property(p => p.OrganizationId)
            .IsRequired();

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

        // Relationships
        builder.HasOne(p => p.Product)
            .WithMany()
            .HasForeignKey(p => p.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
