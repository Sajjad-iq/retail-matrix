using Domains.Stock.Entities;
using Domains.Shared.Base;

namespace Domains.Stock.Repositories;

/// <summary>
/// Repository interface for ProductStock entity
/// </summary>
public interface IProductStockRepository : IRepository<ProductStock>
{
    // Single item queries
    Task<ProductStock?> GetByPackagingAsync(
        Guid packagingId,
        Guid organizationId,
        Guid? locationId = null,
        CancellationToken cancellationToken = default);

    // Paginated queries
    Task<PagedResult<ProductStock>> GetByOrganizationAsync(
        Guid organizationId,
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
        Guid organizationId,
        List<Guid> packagingIds,
        CancellationToken cancellationToken = default);
}
