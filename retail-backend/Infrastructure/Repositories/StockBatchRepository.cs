using Domains.Stock.Entities;
using Domains.Products.Entities;
using Domains.Stock.Enums;
using Domains.Products.Enums;
using Domains.Stock.Repositories;
using Domains.Shared.Base;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class StockBatchRepository : Repository<StockBatch>, IStockBatchRepository
{
    public StockBatchRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<StockBatch>> GetByStockIdAsync(Guid stockId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(b => b.ProductStockId == stockId)
            .OrderBy(b => b.InsertDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<StockBatch>> GetByBatchNumberAsync(string batchNumber, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(b => b.BatchNumber == batchNumber)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<StockBatch>> GetExpiringBatchesAsync(Guid organizationId, int daysThreshold, CancellationToken cancellationToken = default)
    {
        var thresholdDate = DateTime.UtcNow.AddDays(daysThreshold);

        return await _dbSet
            .AsNoTracking()
            .Include(b => b.ProductStock)
            .Where(b => b.ProductStock!.OrganizationId == organizationId &&
                       b.ExpirationDate.HasValue &&
                       b.ExpirationDate.Value <= thresholdDate &&
                       b.ExpirationDate.Value > DateTime.UtcNow &&
                       b.RemainingQuantity > 0)
            .OrderBy(b => b.ExpirationDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<StockBatch>> GetExpiredBatchesAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(b => b.ProductStock)
            .Where(b => b.ProductStock!.OrganizationId == organizationId &&
                       b.ExpirationDate.HasValue &&
                       b.ExpirationDate.Value <= DateTime.UtcNow &&
                       b.RemainingQuantity > 0)
            .OrderBy(b => b.ExpirationDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<StockBatch>> GetByConditionAsync(Guid organizationId, StockCondition condition, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(b => b.ProductStock)
            .Where(b => b.ProductStock!.OrganizationId == organizationId && b.Condition == condition)
            .OrderBy(b => b.InsertDate)
            .ToListAsync(cancellationToken);
    }
}
