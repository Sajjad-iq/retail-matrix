using Domains.Stock.Entities;
using Domains.Stock.Enums;
using Domains.Shared.Base;

namespace Domains.Stock.Repositories;

/// <summary>
/// Repository interface for StockMovement entity
/// </summary>
public interface IStockMovementRepository : IRepository<StockMovement>
{
    Task<IEnumerable<StockMovement>> GetByPackagingAsync(Guid packagingId, CancellationToken cancellationToken = default);
    Task<IEnumerable<StockMovement>> GetByDateRangeAsync(Guid organizationId, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<StockMovement>> GetByTypeAsync(Guid organizationId, StockMovementType type, CancellationToken cancellationToken = default);
    Task<IEnumerable<StockMovement>> GetByReferenceNumberAsync(string referenceNumber, CancellationToken cancellationToken = default);
}
