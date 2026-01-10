using Domains.Entities;
using Domains.Shared;

namespace Domains.Repositories;

/// <summary>
/// Repository interface for ProductStock entity
/// </summary>
public interface IProductStockRepository
{
    // Single item queries
    Task<ProductStock?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ProductStock?> GetByPackagingAndLocationAsync(Guid packagingId, Guid? locationId, CancellationToken cancellationToken = default);

    // Paginated queries
    Task<PagedResult<ProductStock>> GetByPackagingIdAsync(
        Guid packagingId,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default);

    Task<PagedResult<ProductStock>> GetByLocationAsync(
        Guid locationId,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default);

    Task<PagedResult<ProductStock>> GetByOrganizationAsync(
        Guid organizationId,
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

    Task<PagedResult<ProductStock>> GetExpiredStockAsync(
        Guid organizationId,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default);

    Task<PagedResult<ProductStock>> GetExpiringSoonStockAsync(
        Guid organizationId,
        int daysThreshold,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default);

    // CRUD operations
    Task<ProductStock> AddAsync(ProductStock stock, CancellationToken cancellationToken = default);
    Task<ProductStock> UpdateAsync(ProductStock stock, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    // Batch loading method (returns dictionary, not paginated)
    Task<Dictionary<Guid, List<ProductStock>>> GetByPackagingIdsAsync(
        List<Guid> packagingIds,
        CancellationToken cancellationToken = default);
}
