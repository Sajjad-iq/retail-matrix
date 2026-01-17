using Domains.Inventory.Entities;
using Domains.Inventory.Repositories;
using Domains.Shared.Base;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

/// <summary>
/// Entity Framework Core implementation of IStockRepository
/// </summary>
public class StockRepository : Repository<Stock>, IStockRepository
{
    public StockRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Stock?> GetByPackagingAsync(
        Guid packagingId,
        Guid organizationId,
        Guid inventoryId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(s =>
                s.ProductPackagingId == packagingId &&
                s.OrganizationId == organizationId &&
                s.InventoryId == inventoryId,
                cancellationToken);
    }

    public async Task<PagedResult<Stock>> GetByOrganizationAsync(
        Guid organizationId,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .AsNoTracking()
            .Where(s => s.OrganizationId == organizationId)
            .OrderBy(s => s.ProductPackagingId);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip(pagingParams.Skip)
            .Take(pagingParams.Take)
            .ToListAsync(cancellationToken);

        return new PagedResult<Stock>(items, totalCount, pagingParams.PageNumber, pagingParams.PageSize);
    }

    public async Task<PagedResult<Stock>> GetByInventoryAsync(
        Guid inventoryId,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .AsNoTracking()
            .Where(s => s.InventoryId == inventoryId)
            .OrderBy(s => s.ProductPackagingId);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip(pagingParams.Skip)
            .Take(pagingParams.Take)
            .ToListAsync(cancellationToken);

        return new PagedResult<Stock>(items, totalCount, pagingParams.PageNumber, pagingParams.PageSize);
    }

    public async Task<PagedResult<Stock>> GetLowStockItemsAsync(
        Guid organizationId,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default)
    {
        // Query for low stock items (available > 0 and <= 10)
        var query = _dbSet
            .AsNoTracking()
            .Where(s => s.OrganizationId == organizationId)
            .Where(s => (s.Quantity - s.ReservedQuantity) > 0 && (s.Quantity - s.ReservedQuantity) <= 10)
            .OrderBy(s => s.Quantity - s.ReservedQuantity)
            .ThenBy(s => s.ProductPackagingId);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip(pagingParams.Skip)
            .Take(pagingParams.Take)
            .ToListAsync(cancellationToken);

        return new PagedResult<Stock>(items, totalCount, pagingParams.PageNumber, pagingParams.PageSize);
    }

    public async Task<PagedResult<Stock>> GetOutOfStockItemsAsync(
        Guid organizationId,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default)
    {
        // Query for out of stock items (available == 0)
        var query = _dbSet
            .AsNoTracking()
            .Where(s => s.OrganizationId == organizationId)
            .Where(s => (s.Quantity - s.ReservedQuantity) == 0)
            .OrderBy(s => s.ProductPackagingId);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip(pagingParams.Skip)
            .Take(pagingParams.Take)
            .ToListAsync(cancellationToken);

        return new PagedResult<Stock>(items, totalCount, pagingParams.PageNumber, pagingParams.PageSize);
    }

    public async Task<List<Stock>> GetByPackagingIdsAsync(
        Guid organizationId,
        List<Guid> packagingIds,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(s => s.OrganizationId == organizationId && packagingIds.Contains(s.ProductPackagingId))
            .ToListAsync(cancellationToken);
    }
}
