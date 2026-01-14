using Domains.Stock.Entities;
using Domains.Stock.Enums;
using Domains.Shared.Base;

namespace Domains.Stock.Repositories;

/// <summary>
/// Repository interface for StockBatch entity
/// </summary>
public interface IStockBatchRepository : IRepository<StockBatch>
{
    Task<IEnumerable<StockBatch>> GetByStockIdAsync(Guid stockId, CancellationToken cancellationToken = default);
    Task<IEnumerable<StockBatch>> GetByBatchNumberAsync(string batchNumber, CancellationToken cancellationToken = default);
    Task<IEnumerable<StockBatch>> GetExpiringBatchesAsync(Guid organizationId, int daysThreshold, CancellationToken cancellationToken = default);
    Task<IEnumerable<StockBatch>> GetExpiredBatchesAsync(Guid organizationId, CancellationToken cancellationToken = default);
    Task<IEnumerable<StockBatch>> GetByConditionAsync(Guid organizationId, StockCondition condition, CancellationToken cancellationToken = default);
}
