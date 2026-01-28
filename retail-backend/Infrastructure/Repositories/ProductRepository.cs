using Domains.Products.Entities;
using Domains.Products.Repositories;
using Domains.Products.Enums;
using Domains.Shared.Base;
using Domains.Sales.Enums;
using Domains.Products.Enums;
using Domains.Products.Models;
using Domains.Organizations.Enums;
using Domains.Users.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

/// <summary>
/// Entity Framework Core implementation of IProductRepository
/// </summary>
public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(ApplicationDbContext context) : base(context)
    {
    }
    public async Task<PagedResult<Product>> GetListAsync(
        Guid organizationId,
        ProductFilter filter,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .AsNoTracking()
            .Include(p => p.Category)
            .Where(p => p.OrganizationId == organizationId);

        // Filter by Ids
        if (filter.Ids != null && filter.Ids.Any())
        {
            query = query.Where(p => filter.Ids.Contains(p.Id));
        }

        // Filter by Category
        if (filter.CategoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == filter.CategoryId.Value);
        }

        if (filter.CategoryIds != null && filter.CategoryIds.Any())
        {
            query = query.Where(p => p.CategoryId.HasValue && filter.CategoryIds.Contains(p.CategoryId.Value));
        }

        // Filter by Status
        if (filter.Status.HasValue)
        {
            query = query.Where(p => p.Status == filter.Status.Value);
        }

        // Filter by Name
        if (!string.IsNullOrWhiteSpace(filter.Name))
        {
            query = query.Where(p => p.Name.Contains(filter.Name));
        }

        // Filter by SearchTerm (Name only)
        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var term = filter.SearchTerm.ToLower();
            query = query.Where(p => p.Name.ToLower().Contains(term));
        }

        // Filter by Barcode - would need to be handled differently or removed
        // since packagings are no longer loaded with products
        if (!string.IsNullOrWhiteSpace(filter.Barcode))
        {
            // Skip barcode filter for products list, handle at packaging level instead
        }

        query = query.OrderByDescending(p => p.InsertDate);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip(pagingParams.Skip)
            .Take(pagingParams.Take)
            .ToListAsync(cancellationToken);

        return new PagedResult<Product>(items, totalCount, pagingParams.PageNumber, pagingParams.PageSize);
    }


}
