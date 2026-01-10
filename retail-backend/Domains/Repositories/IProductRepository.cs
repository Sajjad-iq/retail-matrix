using Domains.Entities;
using Domains.Enums;

namespace Domains.Repositories;

/// <summary>
/// Repository interface for Product entity
/// </summary>
public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Product?> GetByBarcodeAsync(string barcode, CancellationToken cancellationToken = default);
    Task<List<Product>> GetByOrganizationAsync(Guid organizationId, CancellationToken cancellationToken = default);
    Task<List<Product>> GetByStatusAsync(ProductStatus status, Guid organizationId, CancellationToken cancellationToken = default);
    Task<List<Product>> GetLowStockProductsAsync(Guid organizationId, CancellationToken cancellationToken = default);
    Task<List<Product>> GetOutOfStockProductsAsync(Guid organizationId, CancellationToken cancellationToken = default);
    Task<bool> ExistsByBarcodeAsync(string barcode, CancellationToken cancellationToken = default);
    Task<Product> AddAsync(Product product, CancellationToken cancellationToken = default);
    Task<Product> UpdateAsync(Product product, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
