using Domains.Stock.Entities;
using Domains.Stock.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for Location entity (storage spots)
/// </summary>
public class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        // Table name
        builder.ToTable("Locations");

        // Primary key
        builder.HasKey(l => l.Id);

        // Indexes
        builder.HasIndex(l => l.OrganizationId)
            .HasDatabaseName("IX_Locations_Organization");

        builder.HasIndex(l => new { l.Code, l.OrganizationId })
            .IsUnique()
            .HasDatabaseName("IX_Locations_Code_Organization");

        builder.HasIndex(l => l.Type)
            .HasDatabaseName("IX_Locations_Type");

        builder.HasIndex(l => l.ParentId)
            .HasDatabaseName("IX_Locations_Parent");

        // Properties
        builder.Property(l => l.Id)
            .ValueGeneratedNever();

        builder.Property(l => l.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(l => l.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(l => l.Type)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(l => l.OrganizationId)
            .IsRequired();

        builder.Property(l => l.ParentId)
            .IsRequired(false);

        builder.Property(l => l.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Self-referencing relationship (hierarchical)
        builder.HasOne(l => l.Parent)
            .WithMany(l => l.Children)
            .HasForeignKey(l => l.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Soft delete
        builder.Property(l => l.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // Audit fields
        builder.Property(l => l.InsertDate)
            .IsRequired();

        builder.Property(l => l.UpdateDate)
            .IsRequired();

        builder.Property(l => l.DeletedAt)
            .IsRequired(false);
    }
}
