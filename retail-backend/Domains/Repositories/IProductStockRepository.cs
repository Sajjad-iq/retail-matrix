using Domains.Entities;

namespace Domains.Repositories;

/// <summary>
/// Repository interface for ProductStock entity
/// </summary>
public interface IProductStockRepository
{
    Task<ProductStock?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<ProductStock>> GetByPackagingIdAsync(Guid packagingId, CancellationToken cancellationToken = default);
    Task<ProductStock?> GetByPackagingAndLocationAsync(Guid packagingId, Guid? locationId, CancellationToken cancellationToken = default);
    Task<List<ProductStock>> GetByLocationAsync(Guid locationId, CancellationToken cancellationToken = default);
    Task<List<ProductStock>> GetByOrganizationAsync(Guid organizationId, CancellationToken cancellationToken = default);
    Task<List<ProductStock>> GetLowStockItemsAsync(Guid organizationId, CancellationToken cancellationToken = default);
    Task<List<ProductStock>> GetOutOfStockItemsAsync(Guid organizationId, CancellationToken cancellationToken = default);
    Task<List<ProductStock>> GetExpiredStockAsync(Guid organizationId, CancellationToken cancellationToken = default);
    Task<List<ProductStock>> GetExpiringSoonStockAsync(Guid organizationId, int daysThreshold = 30, CancellationToken cancellationToken = default);
    Task<ProductStock> AddAsync(ProductStock stock, CancellationToken cancellationToken = default);
    Task<ProductStock> UpdateAsync(ProductStock stock, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
