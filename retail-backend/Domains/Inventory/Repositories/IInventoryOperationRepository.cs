using Domains.Inventory.Entities;
using Domains.Inventory.Models;
using Domains.Shared.Base;

namespace Domains.Inventory.Repositories;

/// <summary>
/// Repository interface for InventoryOperation entity
/// </summary>
public interface IInventoryOperationRepository : IRepository<InventoryOperation>
{
    Task<PagedResult<InventoryOperation>> GetListAsync(
        Guid organizationId,
        InventoryOperationFilter filter,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default);
}
