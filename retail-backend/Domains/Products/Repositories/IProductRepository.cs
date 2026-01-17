using Domains.Products.Entities;
using Domains.Products.Enums;
using Domains.Shared.Base;

namespace Domains.Products.Repositories;

/// <summary>
/// Repository interface for Product entity
/// </summary>
public interface IProductRepository : IRepository<Product>
{
    // Paginated queries
    Task<PagedResult<Product>> GetByOrganizationAsync(
        Guid organizationId,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default);

    Task<PagedResult<Product>> GetByStatusAsync(
        ProductStatus status,
        Guid organizationId,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default);
}
