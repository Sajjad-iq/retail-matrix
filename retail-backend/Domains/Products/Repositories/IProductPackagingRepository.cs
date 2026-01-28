using Domains.Products.Entities;
using Domains.Products.Models;
using Domains.Shared.Base;

namespace Domains.Products.Repositories;

/// <summary>
/// Repository interface for ProductPackaging entity
/// </summary>
public interface IProductPackagingRepository : IRepository<ProductPackaging>
{
    // Paginated queries
    Task<PagedResult<ProductPackaging>> GetListAsync(
        Guid organizationId,
        ProductPackagingFilter filter,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default);

    // Single item queries
    Task<ProductPackaging?> GetByIdWithProductAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ProductPackaging?> GetByBarcodeAsync(string barcode, CancellationToken cancellationToken = default);
    Task<ProductPackaging?> GetByBarcodeWithProductAsync(string barcode, CancellationToken cancellationToken = default);

    // Existence checks
    Task<bool> ExistsByBarcodeAsync(string barcode, CancellationToken cancellationToken = default);
}
