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

        builder.HasIndex(p => p.IsDefault)
            .HasDatabaseName("IX_ProductPackagings_IsDefault");

        // Properties configuration
        builder.Property(p => p.Id)
            .ValueGeneratedNever(); // Generated in domain

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        // Configure Barcode value object using HasConversion
        builder.Property(p => p.Barcode)
            .HasConversion(
                b => b.Value,
                v => Barcode.Create(v))
            .HasColumnName("Barcode")
            .HasMaxLength(13);

        builder.Property(p => p.UnitsPerPackage)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(p => p.UnitOfMeasure)
            .IsRequired()
            .HasConversion<string>();

        // Configure Price value object using ComplexProperty
        builder.ComplexProperty(p => p.SellingPrice, priceBuilder =>
        {
            priceBuilder.Property(price => price.Amount)
                .HasColumnName("SellingPrice")
                .IsRequired()
                .HasPrecision(18, 2);

            priceBuilder.Property(price => price.Currency)
                .HasColumnName("SellingCurrency")
                .IsRequired()
                .HasMaxLength(3);
        });

        // Configure Discount value object using ComplexProperty
        builder.ComplexProperty(p => p.Discount, discountBuilder =>
        {
            discountBuilder.IsRequired();

            discountBuilder.Property(d => d.Value)
                .HasColumnName("DiscountValue")
                .HasPrecision(18, 2);

            discountBuilder.Property(d => d.Type)
                .HasColumnName("DiscountType")
                .HasConversion<string>()
                .HasMaxLength(20);
        });

        builder.Property(p => p.IsDefault)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(p => p.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasDefaultValue(ProductStatus.Active);

        builder.Property(p => p.Description)
            .HasMaxLength(500);

        builder.Property(p => p.ImageUrls)
            .HasConversion(
                v => string.Join(",", v),  // Convert list to comma-separated string
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())  // Convert back to list
            .HasMaxLength(2000);  // Increased for multiple URLs

        builder.Property(p => p.Dimensions)
            .HasMaxLength(100);

        // Configure Weight as owned entity (supports null)
        builder.OwnsOne(p => p.Weight, weightBuilder =>
        {
            weightBuilder.Property(weight => weight.Value)
                .HasColumnName("Weight")
                .HasPrecision(18, 3);

            weightBuilder.Property(weight => weight.Unit)
                .HasColumnName("WeightUnit")
                .HasConversion<string>();
        });

        builder.Property(p => p.Color)
            .HasMaxLength(50);

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
            .WithMany(prod => prod.Packagings)
            .HasForeignKey(p => p.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
