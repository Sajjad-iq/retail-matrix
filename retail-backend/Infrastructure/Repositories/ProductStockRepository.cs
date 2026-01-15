using Domains.Stock.Entities;
using Domains.Products.Entities;
using Domains.Stock.Repositories;
using Domains.Shared.Base;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

/// <summary>
/// Entity Framework Core implementation of IProductStockRepository
/// </summary>
public class ProductStockRepository : Repository<ProductStock>, IProductStockRepository
{
    public ProductStockRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<ProductStock?> GetByPackagingAndLocationAsync(
        Guid packagingId,
        Guid? locationId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(s => s.Batches)  // Eager load batches for computed properties
            .FirstOrDefaultAsync(s => s.ProductPackagingId == packagingId && s.LocationId == locationId, cancellationToken);
    }

    public async Task<PagedResult<ProductStock>> GetByProductPackagingIdAsync(
        Guid packagingId,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .AsNoTracking()
            .Include(s => s.Batches)  // Eager load batches for computed properties
            .Where(s => s.ProductPackagingId == packagingId)
            .OrderBy(s => s.LocationId);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip(pagingParams.Skip)
            .Take(pagingParams.Take)
            .ToListAsync(cancellationToken);

        return new PagedResult<ProductStock>(items, totalCount, pagingParams.PageNumber, pagingParams.PageSize);
    }

    public async Task<PagedResult<ProductStock>> GetByLocationAsync(
        Guid locationId,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .AsNoTracking()
            .Include(s => s.Batches)  // Eager load batches for computed properties
            .Where(s => s.LocationId == locationId)
            .OrderBy(s => s.ProductPackagingId);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip(pagingParams.Skip)
            .Take(pagingParams.Take)
            .ToListAsync(cancellationToken);

        return new PagedResult<ProductStock>(items, totalCount, pagingParams.PageNumber, pagingParams.PageSize);
    }

    public async Task<PagedResult<ProductStock>> GetLowStockItemsAsync(
        Guid organizationId,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default)
    {
        // Load all stocks with batches, then filter in memory since GoodStock is computed
        var allStocks = await _dbSet
            .AsNoTracking()
            .Include(s => s.Batches)
            .Where(s => s.OrganizationId == organizationId)
            .ToListAsync(cancellationToken);

        // Filter and sort in memory based on computed properties
        var lowStockItems = allStocks
            .Where(s => s.AvailableStock > 0 && s.AvailableStock <= 10)
            .OrderBy(s => s.AvailableStock)
            .ThenBy(s => s.ProductPackagingId)
            .ToList();

        var totalCount = lowStockItems.Count;
        var items = lowStockItems
            .Skip(pagingParams.Skip)
            .Take(pagingParams.Take)
            .ToList();

        return new PagedResult<ProductStock>(items, totalCount, pagingParams.PageNumber, pagingParams.PageSize);
    }

    public async Task<PagedResult<ProductStock>> GetOutOfStockItemsAsync(
        Guid organizationId,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default)
    {
        // Load all stocks with batches, then filter in memory since AvailableStock is computed
        var allStocks = await _dbSet
            .AsNoTracking()
            .Include(s => s.Batches)
            .Where(s => s.OrganizationId == organizationId)
            .ToListAsync(cancellationToken);

        // Filter in memory based on computed property
        var outOfStockItems = allStocks
            .Where(s => s.AvailableStock == 0)
            .OrderBy(s => s.ProductPackagingId)
            .ToList();

        var totalCount = outOfStockItems.Count;
        var items = outOfStockItems
            .Skip(pagingParams.Skip)
            .Take(pagingParams.Take)
            .ToList();

        return new PagedResult<ProductStock>(items, totalCount, pagingParams.PageNumber, pagingParams.PageSize);
    }

    public async Task<List<ProductStock>> GetByPackagingIdsAsync(
        List<Guid> packagingIds,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(s => s.Batches)  // Eager load batches for computed properties
            .Where(s => packagingIds.Contains(s.ProductPackagingId))
            .ToListAsync(cancellationToken);
    }
}
