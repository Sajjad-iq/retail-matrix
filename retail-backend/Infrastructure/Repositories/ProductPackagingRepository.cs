using Domains.Entities;
using Domains.Repositories;
using Domains.Shared;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

/// <summary>
/// Entity Framework Core implementation of IProductPackagingRepository
/// </summary>
public class ProductPackagingRepository : IProductPackagingRepository
{
    private readonly ApplicationDbContext _context;

    public ProductPackagingRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    // Single item queries
    public async Task<ProductPackaging?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ProductPackagings
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<ProductPackaging?> GetByBarcodeAsync(string barcode, CancellationToken cancellationToken = default)
    {
        return await _context.ProductPackagings
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Barcode == barcode, cancellationToken);
    }

    public async Task<ProductPackaging?> GetDefaultPackagingAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await _context.ProductPackagings
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.ProductId == productId && p.IsDefault, cancellationToken);
    }

    // Paginated queries
    public async Task<PagedResult<ProductPackaging>> GetByProductIdAsync(
        Guid productId,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default)
    {
        var query = _context.ProductPackagings
            .AsNoTracking()
            .Where(p => p.ProductId == productId)
            .OrderByDescending(p => p.IsDefault)
            .ThenBy(p => p.UnitOfMeasure);

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
        var query = _context.ProductPackagings
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
        var query = _context.ProductPackagings
            .AsNoTracking()
            .Where(p => p.ProductId == productId && p.IsActive)
            .OrderByDescending(p => p.IsDefault);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip(pagingParams.Skip)
            .Take(pagingParams.Take)
            .ToListAsync(cancellationToken);

        return new PagedResult<ProductPackaging>(items, totalCount, pagingParams.PageNumber, pagingParams.PageSize);
    }

    // Existence checks
    public async Task<bool> ExistsByBarcodeAsync(string barcode, CancellationToken cancellationToken = default)
    {
        return await _context.ProductPackagings
            .AsNoTracking()
            .AnyAsync(p => p.Barcode == barcode, cancellationToken);
    }

    // CRUD operations
    public async Task<ProductPackaging> AddAsync(ProductPackaging packaging, CancellationToken cancellationToken = default)
    {
        await _context.ProductPackagings.AddAsync(packaging, cancellationToken);
        return packaging;
    }

    public async Task<ProductPackaging> UpdateAsync(ProductPackaging packaging, CancellationToken cancellationToken = default)
    {
        _context.ProductPackagings.Update(packaging);
        return await Task.FromResult(packaging);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var packaging = await _context.ProductPackagings.FindAsync(new object[] { id }, cancellationToken);

        if (packaging == null)
            return false;

        _context.ProductPackagings.Remove(packaging);
        return true;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
