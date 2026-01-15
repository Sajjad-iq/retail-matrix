using Domains.Inventory.Entities;
using Domains.Shared.Base;

namespace Domains.Inventory.Repositories;

/// <summary>
/// Repository interface for InventoryOperation entity
/// </summary>
public interface IInventoryOperationRepository : IRepository<InventoryOperation>
{
    Task<InventoryOperation?> GetByOperationNumberAsync(
        string operationNumber,
        Guid organizationId,
        CancellationToken cancellationToken = default);

    Task<PagedResult<InventoryOperation>> GetByOrganizationAsync(
        Guid organizationId,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default);

    Task<PagedResult<InventoryOperation>> GetByDateRangeAsync(
        Guid organizationId,
        DateTime fromDate,
        DateTime toDate,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default);

    Task<List<InventoryOperation>> GetByStatusAsync(
        Guid organizationId,
        Enums.InventoryOperationStatus status,
        CancellationToken cancellationToken = default);
}
