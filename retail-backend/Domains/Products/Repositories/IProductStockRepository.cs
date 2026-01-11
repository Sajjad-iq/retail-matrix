using Domains.Products.Entities;
using Domains.Shared.Base;

namespace Domains.Products.Repositories;

/// <summary>
/// Repository interface for ProductStock entity
/// </summary>
public interface IProductStockRepository : IRepository<ProductStock>
{
    // Single item queries
    Task<ProductStock?> GetByPackagingAndLocationAsync(
        Guid packagingId,
        Guid? locationId,
        CancellationToken cancellationToken = default);

    // Paginated queries
    Task<PagedResult<ProductStock>> GetByProductPackagingIdAsync(
        Guid packagingId,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default);

    Task<PagedResult<ProductStock>> GetByLocationAsync(
        Guid locationId,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default);

    Task<PagedResult<ProductStock>> GetLowStockItemsAsync(
        Guid organizationId,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default);

    Task<PagedResult<ProductStock>> GetOutOfStockItemsAsync(
        Guid organizationId,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default);

    // Batch operations
    Task<List<ProductStock>> GetByPackagingIdsAsync(
        List<Guid> packagingIds,
        CancellationToken cancellationToken = default);
}
