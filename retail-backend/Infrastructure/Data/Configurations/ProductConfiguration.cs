using Domains.Products.Entities;
using Domains.Products.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for Product entity
/// </summary>
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        // Table name
        builder.ToTable("Products");

        // Primary key
        builder.HasKey(p => p.Id);

        // Indexes
        builder.HasIndex(p => p.OrganizationId)
            .HasDatabaseName("IX_Products_Organization");

        builder.HasIndex(p => p.Status)
            .HasDatabaseName("IX_Products_Status");

        // Properties configuration
        builder.Property(p => p.Id)
            .ValueGeneratedNever(); // Generated in domain

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Description)
            .HasMaxLength(1000);

        builder.Property(p => p.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasDefaultValue(ProductStatus.Active);

        builder.Property(p => p.OrganizationId)
            .IsRequired();

        builder.Property(p => p.ImageUrl)
            .HasMaxLength(500);

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
