using Domains.Inventory.Enums;
using Domains.Inventory.Models;
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

    public async Task<PagedResult<InventoryEntity>> GetListAsync(
        Guid organizationId,
        InventoryFilter filter,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .AsNoTracking()
            .Where(i => i.OrganizationId == organizationId);

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var searchTerm = filter.SearchTerm.Trim().ToLower();
            query = query.Where(i =>
                i.Name.ToLower().Contains(searchTerm) ||
                i.Code.ToLower().Contains(searchTerm));
        }

        if (filter.Type.HasValue)
        {
            query = query.Where(i => i.Type == filter.Type.Value);
        }

        if (filter.ParentId.HasValue)
        {
            query = query.Where(i => i.ParentId == filter.ParentId.Value);
        }
        else if (filter.ParentId == null && filter.Type == null && string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            // If no filters are applied, default to showing root inventories?
            // Actually, if filter.ParentId is explicitly null, we might want roots. 
            // But if it's just 'not provided', usage pattern matters.
            // For now, let's assume strict filtering. If ParentId is null in filter (default), ignore filtering by ParentId.
            // But if we want to fetch roots, we need to pass ParentId=null explicitly? 
            // Wait, nullable guid default is null. 
            // Let's stick to standard: null means filter not applied.
            // If user wants to filter by ParentId=null, they might need a way to express that.
            // Usually "Get All" list returns flattened list unless tree view is requested.
            // The prompt says "single get list of inventory".
        }

        if (filter.IsActive.HasValue)
        {
            query = query.Where(i => i.IsActive == filter.IsActive.Value);
        }

        query = query.OrderBy(i => i.Code);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip(pagingParams.Skip)
            .Take(pagingParams.Take)
            .ToListAsync(cancellationToken);

        return new PagedResult<InventoryEntity>(items, totalCount, pagingParams.PageNumber, pagingParams.PageSize);
    }
}
