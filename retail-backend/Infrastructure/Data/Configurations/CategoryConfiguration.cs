using Domains.Products.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for Category entity
/// </summary>
public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        // Table name
        builder.ToTable("Categories");

        // Primary key
        builder.HasKey(c => c.Id);

        // Indexes
        builder.HasIndex(c => new { c.Name, c.OrganizationId })
            .IsUnique()
            .HasDatabaseName("IX_Categories_Name_Organization");

        builder.HasIndex(c => c.OrganizationId)
            .HasDatabaseName("IX_Categories_Organization");

        builder.HasIndex(c => c.ParentCategoryId)
            .HasDatabaseName("IX_Categories_Parent");

        // Properties
        builder.Property(c => c.Id)
            .ValueGeneratedNever();

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Description)
            .HasMaxLength(1000);

        builder.Property(c => c.OrganizationId)
            .IsRequired();

        builder.Property(c => c.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Relationships
        builder.HasOne(c => c.ParentCategory)
            .WithMany(c => c.SubCategories)
            .HasForeignKey(c => c.ParentCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Soft delete
        builder.Property(c => c.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // Audit fields
        builder.Property(c => c.InsertDate)
            .IsRequired();

        builder.Property(c => c.UpdateDate)
            .IsRequired();

        builder.Property(c => c.DeletedAt)
            .IsRequired(false);
    }
}
