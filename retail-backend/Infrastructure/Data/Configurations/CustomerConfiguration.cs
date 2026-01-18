using Domains.Users.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for Customer entity
/// </summary>
public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        // Table name
        builder.ToTable("Customers");

        // Primary key
        builder.HasKey(c => c.Id);

        // Indexes
        builder.HasIndex(c => c.OrganizationId)
            .HasDatabaseName("IX_Customers_Organization");

        builder.HasIndex(c => new { c.OrganizationId, c.PhoneNumber })
            .IsUnique()
            .HasDatabaseName("IX_Customers_Organization_Phone");

        builder.HasIndex(c => c.Name)
            .HasDatabaseName("IX_Customers_Name");

        // Properties configuration
        builder.Property(c => c.Id)
            .ValueGeneratedNever();

        builder.Property(c => c.OrganizationId)
            .IsRequired();

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.PhoneNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(c => c.Email)
            .HasMaxLength(100);

        builder.Property(c => c.Address)
            .HasMaxLength(500);

        builder.Property(c => c.Notes)
            .HasMaxLength(2000);

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
