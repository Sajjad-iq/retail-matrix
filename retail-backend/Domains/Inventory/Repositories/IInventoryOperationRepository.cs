using Domains.Inventory.Entities;
using Domains.Shared.Base;

namespace Domains.Inventory.Repositories;

/// <summary>
/// Repository interface for InventoryOperation entity
/// </summary>
public interface IInventoryOperationRepository : IRepository<InventoryOperation>
{
    Task<PagedResult<InventoryOperation>> GetByOrganizationAsync(
        Guid organizationId,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default);
}
