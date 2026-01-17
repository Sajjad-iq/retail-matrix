using Domains.Products.Entities;
using Domains.Products.Repositories;
using Domains.Products.Enums;
using Domains.Shared.Base;
using Domains.Sales.Enums;
using Domains.Products.Enums;
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

    public async Task<PagedResult<Product>> GetByOrganizationAsync(
        Guid organizationId,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .AsNoTracking()
            .Where(p => p.OrganizationId == organizationId)
            .OrderBy(p => p.Id);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip(pagingParams.Skip)
            .Take(pagingParams.Take)
            .ToListAsync(cancellationToken);

        return new PagedResult<Product>(items, totalCount, pagingParams.PageNumber, pagingParams.PageSize);
    }

    public async Task<PagedResult<Product>> GetByStatusAsync(
        ProductStatus status,
        Guid organizationId,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .AsNoTracking()
            .Where(p => p.Status == status && p.OrganizationId == organizationId)
            .OrderBy(p => p.Id);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip(pagingParams.Skip)
            .Take(pagingParams.Take)
            .ToListAsync(cancellationToken);

        return new PagedResult<Product>(items, totalCount, pagingParams.PageNumber, pagingParams.PageSize);
    }
}
