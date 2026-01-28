using Domains.Stocks.Entities;
using Domains.Stocks.Enums;
using Domains.Stocks.Models;
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

    // Paginated queries with filters
    // Paginated queries with filters
    Task<PagedResult<Stock>> GetListAsync(
        Guid organizationId,
        StockFilter filter,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default);

    // Bulk operations
    Task<List<Stock>> GetByPackagingIdsAsync(
        Guid organizationId,
        List<Guid> packagingIds,
        CancellationToken cancellationToken = default);

    // Batch-specific queries
    Task<PagedResult<StockBatch>> GetBatchesListAsync(
        Guid organizationId,
        StockBatchFilter filter,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default);

    Task<StockBatch?> GetBatchByNumberAsync(
        Guid stockId,
        string batchNumber,
        CancellationToken cancellationToken = default);
}
