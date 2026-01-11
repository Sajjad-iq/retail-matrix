using Domains.Entities;
using Domains.Repositories;
using Domains.Shared;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

/// <summary>
/// Entity Framework Core implementation of IProductPackagingRepository
/// </summary>
public class ProductPackagingRepository : Repository<ProductPackaging>, IProductPackagingRepository
{
    public ProductPackagingRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<ProductPackaging?> GetByBarcodeAsync(string barcode, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Barcode == barcode, cancellationToken);
    }

    public async Task<ProductPackaging?> GetDefaultPackagingAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.ProductId == productId && p.IsDefault, cancellationToken);
    }

    public async Task<PagedResult<ProductPackaging>> GetByProductIdAsync(
        Guid productId,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .AsNoTracking()
            .Where(p => p.ProductId == productId)
            .OrderBy(p => p.ProductId);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip(pagingParams.Skip)
            .Take(pagingParams.Take)
            .ToListAsync(cancellationToken);

        return new PagedResult<ProductPackaging>(items, totalCount, pagingParams.PageNumber, pagingParams.PageSize);
    }

    public async Task<PagedResult<ProductPackaging>> GetByOrganizationAsync(
        Guid organizationId,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .AsNoTracking()
            .Where(p => p.OrganizationId == organizationId)
            .OrderBy(p => p.ProductId);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip(pagingParams.Skip)
            .Take(pagingParams.Take)
            .ToListAsync(cancellationToken);

        return new PagedResult<ProductPackaging>(items, totalCount, pagingParams.PageNumber, pagingParams.PageSize);
    }

    public async Task<PagedResult<ProductPackaging>> GetActivePackagingsAsync(
        Guid productId,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .AsNoTracking()
            .Where(p => p.ProductId == productId && p.IsActive)
            .OrderBy(p => p.ProductId);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip(pagingParams.Skip)
            .Take(pagingParams.Take)
            .ToListAsync(cancellationToken);

        return new PagedResult<ProductPackaging>(items, totalCount, pagingParams.PageNumber, pagingParams.PageSize);
    }

    public async Task<bool> ExistsByBarcodeAsync(string barcode, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .AnyAsync(p => p.Barcode == barcode, cancellationToken);
    }
}
