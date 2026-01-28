using Domains.Inventory.Entities;
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

    public async Task<PagedResult<InventoryOperation>> GetByOrganizationAsync(
        Guid organizationId,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .AsNoTracking()
            .Include(o => o.User)
            .Include(o => o.SourceInventory)
            .Include(o => o.DestinationInventory)
            .Where(o => o.OrganizationId == organizationId)
            .OrderByDescending(o => o.OperationDate);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip(pagingParams.Skip)
            .Take(pagingParams.Take)
            .ToListAsync(cancellationToken);

        return new PagedResult<InventoryOperation>(items, totalCount, pagingParams.PageNumber, pagingParams.PageSize);
    }
}
