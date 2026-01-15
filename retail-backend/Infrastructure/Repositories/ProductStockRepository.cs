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

    public async Task<ProductStock?> GetByPackagingAsync(
        Guid packagingId,
        Guid organizationId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.ProductPackagingId == packagingId && s.OrganizationId == organizationId, cancellationToken);
    }

    public async Task<PagedResult<ProductStock>> GetByOrganizationAsync(
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

        return new PagedResult<ProductStock>(items, totalCount, pagingParams.PageNumber, pagingParams.PageSize);
    }

    public async Task<PagedResult<ProductStock>> GetLowStockItemsAsync(
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

        return new PagedResult<ProductStock>(items, totalCount, pagingParams.PageNumber, pagingParams.PageSize);
    }

    public async Task<PagedResult<ProductStock>> GetOutOfStockItemsAsync(
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

        return new PagedResult<ProductStock>(items, totalCount, pagingParams.PageNumber, pagingParams.PageSize);
    }

    public async Task<List<ProductStock>> GetByPackagingIdsAsync(
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
