using Domains.Entities;
using Domains.Shared;

namespace Domains.Repositories;

/// <summary>
/// Repository interface for ProductPackaging entity
/// </summary>
public interface IProductPackagingRepository
{
    // Single item queries
    Task<ProductPackaging?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ProductPackaging?> GetByBarcodeAsync(string barcode, CancellationToken cancellationToken = default);
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

    // CRUD operations
    Task<ProductPackaging> AddAsync(ProductPackaging packaging, CancellationToken cancellationToken = default);
    Task<ProductPackaging> UpdateAsync(ProductPackaging packaging, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
