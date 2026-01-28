using Domains.Products.Entities;
using Domains.Products.Enums;
using Domains.Shared.Base;

using Domains.Products.Models;

namespace Domains.Products.Repositories;

/// <summary>
/// Repository interface for Product entity
/// </summary>
public interface IProductRepository : IRepository<Product>
{
    // Paginated queries
    Task<PagedResult<Product>> GetListAsync(
        Guid organizationId,
        ProductFilter filter,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default);


}
