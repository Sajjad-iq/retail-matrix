using Domains.Organizations.Entities;
using Domains.Organizations.Enums;
using Domains.Shared.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework Core configuration for Organization entity
/// </summary>
public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.ToTable("Organizations");

        // Primary Key
        builder.HasKey(o => o.Id);

        // Properties
        builder.Property(o => o.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(o => o.Domain)
            .IsRequired()
            .HasMaxLength(253);

        builder.Property(o => o.Description)
            .HasMaxLength(1000);

        builder.Property(o => o.Address)
            .HasMaxLength(500);

        builder.Property(o => o.Phone)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(o => o.Status)
            .IsRequired()
            .HasConversion<string>(); // Store enum as string in database

        builder.Property(o => o.CreatedBy)
            .IsRequired();

        builder.Property(o => o.LogoUrl)
            .HasMaxLength(500);

        builder.Property(o => o.InsertDate)
            .IsRequired();

        builder.Property(o => o.UpdateDate)
            .IsRequired();

        builder.Property(o => o.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(o => o.DeletedAt);

        // Indexes
        builder.HasIndex(o => o.Domain)
            .IsUnique()
            .HasDatabaseName("IX_Organizations_Domain");

        builder.HasIndex(o => o.Status)
            .HasDatabaseName("IX_Organizations_Status");

        builder.HasIndex(o => o.CreatedBy)
            .HasDatabaseName("IX_Organizations_CreatedBy");

        builder.HasIndex(o => o.IsDeleted)
            .HasDatabaseName("IX_Organizations_IsDeleted");

        // Query Filter for soft delete
        builder.HasQueryFilter(o => !o.IsDeleted);
    }
}
