using Domains.Inventory.Entities;
using Domains.Inventory.Models;
using Domains.Inventory.Repositories;
using Domains.Shared.Base;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

/// <summary>
/// Entity Framework Core implementation of IInventoryOperationRepository
/// </summary>
public class InventoryOperationRepository : Repository<InventoryOperation>, IInventoryOperationRepository
{
    public InventoryOperationRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<PagedResult<InventoryOperation>> GetListAsync(
        Guid organizationId,
        InventoryOperationFilter filter,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .AsNoTracking()
            .Include(o => o.User)
            .Include(o => o.SourceInventory)
            .Include(o => o.DestinationInventory)
            .Where(o => o.OrganizationId == organizationId);

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var searchTerm = filter.SearchTerm.Trim().ToLower();
            query = query.Where(o =>
                (o.Notes != null && o.Notes.ToLower().Contains(searchTerm)) ||
                o.OperationNumber.ToLower().Contains(searchTerm));
        }

        if (filter.Status.HasValue)
        {
            query = query.Where(o => o.Status == filter.Status.Value);
        }

        if (filter.Type.HasValue)
        {
            query = query.Where(o => o.OperationType == filter.Type.Value);
        }

        if (filter.StartDate.HasValue)
        {
            query = query.Where(o => o.OperationDate >= filter.StartDate.Value);
        }

        if (filter.EndDate.HasValue)
        {
            query = query.Where(o => o.OperationDate <= filter.EndDate.Value);
        }

        if (filter.SourceInventoryId.HasValue)
        {
            query = query.Where(o => o.SourceInventoryId == filter.SourceInventoryId.Value);
        }

        if (filter.DestinationInventoryId.HasValue)
        {
            query = query.Where(o => o.DestinationInventoryId == filter.DestinationInventoryId.Value);
        }

        query = query.OrderByDescending(o => o.OperationDate);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip(pagingParams.Skip)
            .Take(pagingParams.Take)
            .ToListAsync(cancellationToken);

        return new PagedResult<InventoryOperation>(items, totalCount, pagingParams.PageNumber, pagingParams.PageSize);
    }
}
