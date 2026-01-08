using Domains.Repositories;
using Domains.Services;
using Infrastructure.Repositories;
using Infrastructure.Security;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

/// <summary>
/// Extension methods for registering infrastructure services
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Registers all infrastructure services (repositories, security, etc.)
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // Domain Services
        services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();

        // Add more repositories here as you create them:
        // services.AddScoped<IProductRepository, ProductRepository>();
        // services.AddScoped<IOrderRepository, OrderRepository>();

        return services;
    }
}
