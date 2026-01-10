using Domains.Entities;
using Domains.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

/// <summary>
/// Entity Framework Core implementation of IProductStockRepository
/// </summary>
public class ProductStockRepository : IProductStockRepository
{
    private readonly ApplicationDbContext _context;

    public ProductStockRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ProductStock?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ProductStocks
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<List<ProductStock>> GetByPackagingIdAsync(Guid packagingId, CancellationToken cancellationToken = default)
    {
        return await _context.ProductStocks
            .AsNoTracking()
            .Where(s => s.ProductPackagingId == packagingId)
            .OrderBy(s => s.LocationId)
            .ToListAsync(cancellationToken);
    }

    public async Task<ProductStock?> GetByPackagingAndLocationAsync(Guid packagingId, Guid? locationId, CancellationToken cancellationToken = default)
    {
        return await _context.ProductStocks
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.ProductPackagingId == packagingId && s.LocationId == locationId, cancellationToken);
    }

    public async Task<List<ProductStock>> GetByLocationAsync(Guid locationId, CancellationToken cancellationToken = default)
    {
        return await _context.ProductStocks
            .AsNoTracking()
            .Where(s => s.LocationId == locationId)
            .OrderBy(s => s.ProductPackagingId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<ProductStock>> GetByOrganizationAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        return await _context.ProductStocks
            .AsNoTracking()
            .Where(s => s.OrganizationId == organizationId)
            .OrderBy(s => s.ProductPackagingId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<ProductStock>> GetLowStockItemsAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        return await _context.ProductStocks
            .AsNoTracking()
            .Include(s => s.Packaging)
            .Where(s => s.OrganizationId == organizationId
                     && s.CurrentStock - s.ReservedStock > 0
                     && s.CurrentStock - s.ReservedStock <= s.Packaging!.ReorderLevel)
            .OrderBy(s => s.CurrentStock - s.ReservedStock)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<ProductStock>> GetOutOfStockItemsAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        return await _context.ProductStocks
            .AsNoTracking()
            .Where(s => s.OrganizationId == organizationId && s.CurrentStock - s.ReservedStock == 0)
            .OrderBy(s => s.ProductPackagingId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<ProductStock>> GetExpiredStockAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await _context.ProductStocks
            .AsNoTracking()
            .Where(s => s.OrganizationId == organizationId
                     && s.ExpirationDate.HasValue
                     && s.ExpirationDate.Value <= now)
            .OrderBy(s => s.ExpirationDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<ProductStock>> GetExpiringSoonStockAsync(Guid organizationId, int daysThreshold = 30, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var threshold = now.AddDays(daysThreshold);

        return await _context.ProductStocks
            .AsNoTracking()
            .Where(s => s.OrganizationId == organizationId
                     && s.ExpirationDate.HasValue
                     && s.ExpirationDate.Value > now
                     && s.ExpirationDate.Value <= threshold)
            .OrderBy(s => s.ExpirationDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<ProductStock> AddAsync(ProductStock stock, CancellationToken cancellationToken = default)
    {
        await _context.ProductStocks.AddAsync(stock, cancellationToken);
        return stock;
    }

    public async Task<ProductStock> UpdateAsync(ProductStock stock, CancellationToken cancellationToken = default)
    {
        _context.ProductStocks.Update(stock);
        return await Task.FromResult(stock);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var stock = await _context.ProductStocks.FindAsync(new object[] { id }, cancellationToken);

        if (stock == null)
            return false;

        _context.ProductStocks.Remove(stock); // Soft delete via SaveChangesAsync override
        return true;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
