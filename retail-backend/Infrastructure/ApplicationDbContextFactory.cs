using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure;

/// <summary>
/// Design-time factory for ApplicationDbContext
/// Used by EF Core tools for migrations without requiring a running database
/// </summary>
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        // Use a dummy connection string for design-time operations
        // The actual connection string will be used at runtime from appsettings.json
        var connectionString = "Server=localhost;Port=3306;Database=retail_matrix;User=root;Password=password;";

        optionsBuilder.UseMySql(
            connectionString,
            new MySqlServerVersion(new Version(8, 0, 21)),
            mySqlOptions => mySqlOptions.EnableRetryOnFailure()
        );

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
