using Domains.Entities;
using Domains.Enums;
using Domains.Shared;

namespace Domains.Repositories;

/// <summary>
/// Repository interface for Product entity
/// </summary>
public interface IProductRepository
{
    // Single item queries
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Product?> GetByBarcodeAsync(string barcode, CancellationToken cancellationToken = default);

    // Paginated queries
    Task<PagedResult<Product>> GetByOrganizationAsync(
        Guid organizationId,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default);

    Task<PagedResult<Product>> GetByStatusAsync(
        ProductStatus status,
        Guid organizationId,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default);

    Task<PagedResult<Product>> GetLowStockProductsAsync(
        Guid organizationId,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default);

    Task<PagedResult<Product>> GetOutOfStockProductsAsync(
        Guid organizationId,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default);

    // Existence checks
    Task<bool> ExistsByBarcodeAsync(string barcode, CancellationToken cancellationToken = default);

    // CRUD operations
    Task<Product> AddAsync(Product product, CancellationToken cancellationToken = default);
    Task<Product> UpdateAsync(Product product, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
