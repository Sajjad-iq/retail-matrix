using Domains.Inventory.Enums;
using Domains.Inventory.Models;
using Domains.Shared.Base;
using InventoryEntity = Domains.Inventory.Entities.Inventory;

namespace Domains.Inventory.Repositories;

/// <summary>
/// Repository interface for Inventory entity (storage locations)
/// </summary>
public interface IInventoryRepository : IRepository<InventoryEntity>
{
    Task<InventoryEntity?> GetByCodeAsync(string code, Guid organizationId, CancellationToken cancellationToken = default);

    Task<PagedResult<InventoryEntity>> GetListAsync(
        Guid organizationId,
        InventoryFilter filter,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default);
}
