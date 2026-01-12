using Domains.Sales.Repositories;
using Domains.Products.Repositories;
using Domains.Organizations.Repositories;
using Domains.Users.Repositories;
using Domains.Payments.Repositories;
using Domains.Users.Services;
using Infrastructure.Data;
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
        services.AddScoped<IOrganizationRepository, OrganizationRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IProductPackagingRepository, ProductPackagingRepository>();
        services.AddScoped<IProductStockRepository, ProductStockRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();

        // Add more repositories here as you create them:
        // services.AddScoped<IOrderRepository, OrderRepository>();

        return services;
    }
}
