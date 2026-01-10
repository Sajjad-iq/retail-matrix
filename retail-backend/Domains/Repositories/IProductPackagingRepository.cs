using Domains.Entities;

namespace Domains.Repositories;

/// <summary>
/// Repository interface for ProductPackaging entity
/// </summary>
public interface IProductPackagingRepository
{
    Task<ProductPackaging?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ProductPackaging?> GetByBarcodeAsync(string barcode, CancellationToken cancellationToken = default);
    Task<List<ProductPackaging>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<ProductPackaging?> GetDefaultPackagingAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<List<ProductPackaging>> GetByOrganizationAsync(Guid organizationId, CancellationToken cancellationToken = default);
    Task<List<ProductPackaging>> GetActivePackagingsAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<bool> ExistsByBarcodeAsync(string barcode, CancellationToken cancellationToken = default);
    Task<ProductPackaging> AddAsync(ProductPackaging packaging, CancellationToken cancellationToken = default);
    Task<ProductPackaging> UpdateAsync(ProductPackaging packaging, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
