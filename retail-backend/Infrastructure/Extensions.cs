using Domains.Sales.Repositories;
using Domains.Stock.Repositories;
using Domains.Products.Repositories;
using Domains.Organizations.Repositories;
using Domains.Users.Repositories;
using Domains.Common.Currency.Repositories;
using Domains.Common.Currency.Services;
using Domains.Users.Services;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Infrastructure.Security;
using Infrastructure.Services;
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
        // Memory Cache
        services.AddMemoryCache();

        // Domain Services
        services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
        services.AddScoped<ICurrencyConversionService, CurrencyConversionService>();

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IOrganizationRepository, OrganizationRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IProductPackagingRepository, ProductPackagingRepository>();
        services.AddScoped<IProductStockRepository, ProductStockRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IStockMovementRepository, StockMovementRepository>();
        services.AddScoped<ICurrencyRepository, CurrencyRepository>();

        // Add more repositories here as you create them:
        // services.AddScoped<IOrderRepository, OrderRepository>();

        return services;
    }
}
