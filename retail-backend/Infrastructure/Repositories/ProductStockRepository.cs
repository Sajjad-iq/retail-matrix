using Domains.Entities;
using Domains.Repositories;
using Domains.Shared;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

/// <summary>
/// Entity Framework Core implementation of IProductStockRepository
/// </summary>
public class ProductStockRepository : IProductStockRepository
{
    private readonly ApplicationDbContext _context;

    public ProductStockRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    // Single item queries
    public async Task<ProductStock?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ProductStocks
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<ProductStock?> GetByPackagingAndLocationAsync(
        Guid packagingId,
        Guid? locationId,
        CancellationToken cancellationToken = default)
    {
        return await _context.ProductStocks
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.ProductPackagingId == packagingId && s.LocationId == locationId, cancellationToken);
    }

    // Paginated queries
    public async Task<PagedResult<ProductStock>> GetByPackagingIdAsync(
        Guid packagingId,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default)
    {
        var query = _context.ProductStocks
            .AsNoTracking()
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
        var query = _context.ProductStocks
            .AsNoTracking()
            .Where(s => s.LocationId == locationId)
            .OrderBy(s => s.ProductPackagingId);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip(pagingParams.Skip)
            .Take(pagingParams.Take)
            .ToListAsync(cancellationToken);

        return new PagedResult<ProductStock>(items, totalCount, pagingParams.PageNumber, pagingParams.PageSize);
    }

    public async Task<PagedResult<ProductStock>> GetByOrganizationAsync(
        Guid organizationId,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default)
    {
        var query = _context.ProductStocks
            .AsNoTracking()
            .Where(s => s.OrganizationId == organizationId)
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
        var query = _context.ProductStocks
            .AsNoTracking()
            .Include(s => s.Packaging)
            .Where(s => s.OrganizationId == organizationId
                     && s.CurrentStock - s.ReservedStock > 0
                     && s.CurrentStock - s.ReservedStock <= s.Packaging!.ReorderLevel)
            .OrderBy(s => s.CurrentStock - s.ReservedStock);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip(pagingParams.Skip)
            .Take(pagingParams.Take)
            .ToListAsync(cancellationToken);

        return new PagedResult<ProductStock>(items, totalCount, pagingParams.PageNumber, pagingParams.PageSize);
    }

    public async Task<PagedResult<ProductStock>> GetOutOfStockItemsAsync(
        Guid organizationId,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default)
    {
        var query = _context.ProductStocks
            .AsNoTracking()
            .Where(s => s.OrganizationId == organizationId && s.CurrentStock - s.ReservedStock == 0)
            .OrderBy(s => s.ProductPackagingId);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip(pagingParams.Skip)
            .Take(pagingParams.Take)
            .ToListAsync(cancellationToken);

        return new PagedResult<ProductStock>(items, totalCount, pagingParams.PageNumber, pagingParams.PageSize);
    }

    public async Task<PagedResult<ProductStock>> GetExpiredStockAsync(
        Guid organizationId,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var query = _context.ProductStocks
            .AsNoTracking()
            .Where(s => s.OrganizationId == organizationId
                     && s.ExpirationDate.HasValue
                     && s.ExpirationDate.Value <= now)
            .OrderBy(s => s.ExpirationDate);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip(pagingParams.Skip)
            .Take(pagingParams.Take)
            .ToListAsync(cancellationToken);

        return new PagedResult<ProductStock>(items, totalCount, pagingParams.PageNumber, pagingParams.PageSize);
    }

    public async Task<PagedResult<ProductStock>> GetExpiringSoonStockAsync(
        Guid organizationId,
        int daysThreshold,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var threshold = now.AddDays(daysThreshold);

        var query = _context.ProductStocks
            .AsNoTracking()
            .Where(s => s.OrganizationId == organizationId
                     && s.ExpirationDate.HasValue
                     && s.ExpirationDate.Value > now
                     && s.ExpirationDate.Value <= threshold)
            .OrderBy(s => s.ExpirationDate);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip(pagingParams.Skip)
            .Take(pagingParams.Take)
            .ToListAsync(cancellationToken);

        return new PagedResult<ProductStock>(items, totalCount, pagingParams.PageNumber, pagingParams.PageSize);
    }

    // CRUD operations
    public async Task<ProductStock> AddAsync(ProductStock stock, CancellationToken cancellationToken = default)
    {
        await _context.ProductStocks.AddAsync(stock, cancellationToken);
        return stock;
    }

    public async Task<ProductStock> UpdateAsync(ProductStock stock, CancellationToken cancellationToken = default)
    {
        _context.ProductStocks.Update(stock);
        return await Task.FromResult(stock);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var stock = await _context.ProductStocks.FindAsync(new object[] { id }, cancellationToken);

        if (stock == null)
            return false;

        _context.ProductStocks.Remove(stock);
        return true;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    // Batch loading method (returns dictionary, not paginated)
    public async Task<Dictionary<Guid, List<ProductStock>>> GetByPackagingIdsAsync(
        List<Guid> packagingIds,
        CancellationToken cancellationToken = default)
    {
        if (packagingIds == null || !packagingIds.Any())
            return new Dictionary<Guid, List<ProductStock>>();

        var stocks = await _context.ProductStocks
            .AsNoTracking()
            .Where(s => packagingIds.Contains(s.ProductPackagingId))
            .OrderBy(s => s.ProductPackagingId)
            .ThenBy(s => s.LocationId)
            .ToListAsync(cancellationToken);

        return stocks
            .GroupBy(s => s.ProductPackagingId)
            .ToDictionary(g => g.Key, g => g.ToList());
    }
}
