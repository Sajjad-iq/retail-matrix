using Domains.Inventory.Enums;
using Domains.Shared.Base;
using InventoryEntity = Domains.Inventory.Entities.Inventory;

namespace Domains.Inventory.Repositories;

/// <summary>
/// Repository interface for Inventory entity (storage locations)
/// </summary>
public interface IInventoryRepository : IRepository<InventoryEntity>
{
    Task<InventoryEntity?> GetByCodeAsync(string code, Guid organizationId, CancellationToken cancellationToken = default);

    Task<PagedResult<InventoryEntity>> GetByOrganizationAsync(
        Guid organizationId,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default);

    Task<PagedResult<InventoryEntity>> GetByTypeAsync(
        Guid organizationId,
        InventoryType type,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default);

    Task<List<InventoryEntity>> GetChildrenAsync(
        Guid parentId,
        CancellationToken cancellationToken = default);

    Task<List<InventoryEntity>> GetRootInventoriesAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default);

    Task<List<InventoryEntity>> GetActiveInventoriesAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default);
}
