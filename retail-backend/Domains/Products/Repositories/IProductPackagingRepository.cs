using Domains.Products.Entities;
using Domains.Shared.Base;

namespace Domains.Products.Repositories;

/// <summary>
/// Repository interface for ProductPackaging entity
/// </summary>
public interface IProductPackagingRepository : IRepository<ProductPackaging>
{
    // Single item queries
    Task<ProductPackaging?> GetByIdWithProductAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ProductPackaging?> GetByBarcodeAsync(string barcode, CancellationToken cancellationToken = default);
    Task<ProductPackaging?> GetByBarcodeWithProductAsync(string barcode, CancellationToken cancellationToken = default);
    Task<ProductPackaging?> GetDefaultPackagingAsync(Guid productId, CancellationToken cancellationToken = default);

    // Paginated queries
    Task<PagedResult<ProductPackaging>> GetByProductIdAsync(
        Guid productId,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default);

    Task<PagedResult<ProductPackaging>> GetByOrganizationAsync(
        Guid organizationId,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default);

    Task<PagedResult<ProductPackaging>> GetActivePackagingsAsync(
        Guid productId,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default);

    // Existence checks
    Task<bool> ExistsByBarcodeAsync(string barcode, CancellationToken cancellationToken = default);
}
