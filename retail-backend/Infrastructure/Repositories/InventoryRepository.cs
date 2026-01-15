using Domains.Inventory.Enums;
using Domains.Inventory.Repositories;
using Domains.Shared.Base;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using InventoryEntity = Domains.Inventory.Entities.Inventory;

namespace Infrastructure.Repositories;

/// <summary>
/// Entity Framework Core implementation of IInventoryRepository
/// </summary>
public class InventoryRepository : Repository<InventoryEntity>, IInventoryRepository
{
    public InventoryRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<InventoryEntity?> GetByCodeAsync(
        string code,
        Guid organizationId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Code == code && i.OrganizationId == organizationId, cancellationToken);
    }

    public async Task<PagedResult<InventoryEntity>> GetByOrganizationAsync(
        Guid organizationId,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .AsNoTracking()
            .Where(i => i.OrganizationId == organizationId)
            .OrderBy(i => i.Code);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip(pagingParams.Skip)
            .Take(pagingParams.Take)
            .ToListAsync(cancellationToken);

        return new PagedResult<InventoryEntity>(items, totalCount, pagingParams.PageNumber, pagingParams.PageSize);
    }

    public async Task<PagedResult<InventoryEntity>> GetByTypeAsync(
        Guid organizationId,
        InventoryType type,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .AsNoTracking()
            .Where(i => i.OrganizationId == organizationId && i.Type == type)
            .OrderBy(i => i.Code);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip(pagingParams.Skip)
            .Take(pagingParams.Take)
            .ToListAsync(cancellationToken);

        return new PagedResult<InventoryEntity>(items, totalCount, pagingParams.PageNumber, pagingParams.PageSize);
    }

    public async Task<List<InventoryEntity>> GetChildrenAsync(
        Guid parentId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(i => i.ParentId == parentId)
            .OrderBy(i => i.Code)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<InventoryEntity>> GetRootInventoriesAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(i => i.OrganizationId == organizationId && i.ParentId == null)
            .OrderBy(i => i.Code)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<InventoryEntity>> GetActiveInventoriesAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(i => i.OrganizationId == organizationId && i.IsActive)
            .OrderBy(i => i.Code)
            .ToListAsync(cancellationToken);
    }
}
