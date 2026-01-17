using Domains.Inventory.Entities;
using Domains.Shared.Base;

namespace Domains.Inventory.Repositories;

/// <summary>
/// Repository interface for Stock entity
/// </summary>
public interface IStockRepository : IRepository<Stock>
{
    // Single item queries
    Task<Stock?> GetByPackagingAsync(
        Guid packagingId,
        Guid organizationId,
        Guid inventoryId,
        CancellationToken cancellationToken = default);

    // Paginated queries
    Task<PagedResult<Stock>> GetByOrganizationAsync(
        Guid organizationId,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default);

    Task<PagedResult<Stock>> GetByInventoryAsync(
        Guid inventoryId,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default);

    Task<PagedResult<Stock>> GetLowStockItemsAsync(
        Guid organizationId,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default);

    Task<PagedResult<Stock>> GetOutOfStockItemsAsync(
        Guid organizationId,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default);

    // Batch operations
    Task<List<Stock>> GetByPackagingIdsAsync(
        Guid organizationId,
        List<Guid> packagingIds,
        CancellationToken cancellationToken = default);
}
