using Domains.Entities;
using Domains.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for User entity
/// </summary>
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // Table name
        builder.ToTable("Users");

        // Primary key
        builder.HasKey(u => u.Id);

        // Indexes
        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName("IX_Users_Email");

        builder.HasIndex(u => u.PhoneNumber)
            .HasDatabaseName("IX_Users_PhoneNumber");

        builder.HasIndex(u => u.MemberOfOrganization)
            .HasDatabaseName("IX_Users_Organization");

        // Properties configuration
        builder.Property(u => u.Id)
            .ValueGeneratedNever(); // Generated in domain

        builder.Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(u => u.PhoneNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(u => u.Address)
            .HasMaxLength(500);

        builder.Property(u => u.Avatar)
            .HasMaxLength(500);

        builder.Property(u => u.Department)
            .HasMaxLength(100);

        builder.Property(u => u.MemberOfOrganization)
            .HasMaxLength(100);

        // AccountType - Store as string
        builder.Property(u => u.AccountType)
            .IsRequired()
            .HasConversion<string>();

        // UserRoles - Store as JSON
        builder.Property(u => u.UserRoles)
            .IsRequired()
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<HashSet<Roles>>(v, (JsonSerializerOptions?)null) ?? new HashSet<Roles>()
            )
            .HasColumnType("json");

        // Boolean fields
        builder.Property(u => u.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(u => u.EmailVerified)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(u => u.PhoneVerified)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(u => u.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // Security fields
        builder.Property(u => u.FailedLoginAttempts)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(u => u.LockedUntil)
            .IsRequired(false);

        // Audit fields
        builder.Property(u => u.InsertDate)
            .IsRequired();

        builder.Property(u => u.UpdateDate)
            .IsRequired();

        builder.Property(u => u.DeletedAt)
            .IsRequired(false);
    }
}
