using Domains.Products.Entities;
using Domains.Products.Enums;
using Domains.Products.Models;
using Domains.Products.Repositories;
using Domains.Shared.Base;
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

    public async Task<PagedResult<ProductPackaging>> GetListAsync(
        Guid organizationId,
        ProductPackagingFilter filter,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .AsNoTracking()
            .Include(p => p.Product)
            .Where(p => p.Product!.OrganizationId == organizationId);

        // Filter by ProductId
        if (filter.ProductId.HasValue)
        {
            query = query.Where(p => p.ProductId == filter.ProductId.Value);
        }

        // Filter by Status
        if (filter.Status.HasValue)
        {
            query = query.Where(p => p.Status == filter.Status.Value);
        }

        // Filter by SearchTerm (Name, Description, or Product Name)
        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var term = filter.SearchTerm.ToLower();
            query = query.Where(p =>
                p.Name.ToLower().Contains(term) ||
                (p.Description != null && p.Description.ToLower().Contains(term)) ||
                (p.Product != null && p.Product.Name.ToLower().Contains(term))
            );
        }

        // Filter by Barcode
        if (!string.IsNullOrWhiteSpace(filter.Barcode))
        {
            query = query.Where(p => p.Barcode == filter.Barcode);
        }

        // Filter by IsDefault
        if (filter.IsDefault.HasValue)
        {
            query = query.Where(p => p.IsDefault == filter.IsDefault.Value);
        }

        query = query.OrderByDescending(p => p.InsertDate);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip(pagingParams.Skip)
            .Take(pagingParams.Take)
            .ToListAsync(cancellationToken);

        return new PagedResult<ProductPackaging>(items, totalCount, pagingParams.PageNumber, pagingParams.PageSize);
    }

    public async Task<ProductPackaging?> GetByIdWithProductAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Product)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<ProductPackaging?> GetByBarcodeAsync(string barcode, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Barcode == barcode, cancellationToken);
    }

    public async Task<ProductPackaging?> GetByBarcodeWithProductAsync(string barcode, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Product)
            .FirstOrDefaultAsync(p => p.Barcode == barcode, cancellationToken);
    }

    public async Task<bool> ExistsByBarcodeAsync(string barcode, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .AnyAsync(p => p.Barcode == barcode, cancellationToken);
    }
}
