using Domains.Inventory.Entities;
using Domains.Inventory.Enums;
using Domains.Shared.Base;

namespace Domains.Inventory.Repositories;

/// <summary>
/// Repository interface for InventoryMovement entity
/// </summary>
public interface IInventoryMovementRepository : IRepository<InventoryMovement>
{
    Task<IEnumerable<InventoryMovement>> GetByPackagingAsync(Guid packagingId, CancellationToken cancellationToken = default);
    Task<IEnumerable<InventoryMovement>> GetByDateRangeAsync(Guid organizationId, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<InventoryMovement>> GetByTypeAsync(Guid organizationId, InventoryMovementType type, CancellationToken cancellationToken = default);
    Task<IEnumerable<InventoryMovement>> GetByReferenceNumberAsync(string referenceNumber, CancellationToken cancellationToken = default);
}
