using Domains.Stocks.Entities;
using Domains.Stocks.Enums;
using Domains.Shared.Base;

namespace Domains.Stocks.Repositories;

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

    Task<Stock?> GetByPackagingWithBatchesAsync(
        Guid packagingId,
        Guid organizationId,
        Guid inventoryId,
        CancellationToken cancellationToken = default);

    Task<Stock?> GetWithBatchesAsync(
        Guid stockId,
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
        int reorderLevel,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default);

    Task<PagedResult<Stock>> GetOutOfStockItemsAsync(
        Guid organizationId,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default);

    // Bulk operations
    Task<List<Stock>> GetByPackagingIdsAsync(
        Guid organizationId,
        List<Guid> packagingIds,
        CancellationToken cancellationToken = default);

    // Batch-specific queries
    Task<PagedResult<StockBatch>> GetExpiredBatchesAsync(
        Guid organizationId,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default);

    Task<PagedResult<StockBatch>> GetNearExpiryBatchesAsync(
        Guid organizationId,
        int daysThreshold,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default);

    Task<PagedResult<StockBatch>> GetBatchesByConditionAsync(
        Guid organizationId,
        StockCondition condition,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default);

    Task<StockBatch?> GetBatchByNumberAsync(
        Guid stockId,
        string batchNumber,
        CancellationToken cancellationToken = default);
}
