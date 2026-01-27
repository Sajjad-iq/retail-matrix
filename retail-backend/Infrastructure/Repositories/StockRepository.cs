using Domains.Stocks.Entities;
using Domains.Stocks.Enums;
using Domains.Stocks.Repositories;
using Domains.Shared.Base;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

/// <summary>
/// Entity Framework Core implementation of IStockRepository
/// </summary>
public class StockRepository : Repository<Stock>, IStockRepository
{
    public StockRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Stock?> GetByPackagingAsync(
        Guid packagingId,
        Guid organizationId,
        Guid inventoryId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(s => s.Batches)
            .FirstOrDefaultAsync(s =>
                s.ProductPackagingId == packagingId &&
                s.OrganizationId == organizationId &&
                s.InventoryId == inventoryId,
                cancellationToken);
    }

    public async Task<Stock?> GetByPackagingWithBatchesAsync(
        Guid packagingId,
        Guid organizationId,
        Guid inventoryId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(s => s.Batches)
            .FirstOrDefaultAsync(s =>
                s.ProductPackagingId == packagingId &&
                s.OrganizationId == organizationId &&
                s.InventoryId == inventoryId,
                cancellationToken);
    }

    public async Task<Stock?> GetWithBatchesAsync(
        Guid stockId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(s => s.Batches)
            .FirstOrDefaultAsync(s => s.Id == stockId, cancellationToken);
    }

    public async Task<PagedResult<Stock>> GetByFiltersAsync(
        Guid organizationId,
        Guid? inventoryId,
        Guid? productPackagingId,
        string? productName,
        bool isLowStock,
        int? reorderLevel,
        bool isOutOfStock,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(s => s.Batches)
            .Include(s => s.ProductPackaging)
            .ThenInclude(p => p.Product)
            .Include(s => s.Inventory)
            .Where(s => s.OrganizationId == organizationId);

        // Apply basic filters
        if (inventoryId.HasValue)
        {
            query = query.Where(s => s.InventoryId == inventoryId.Value);
        }

        if (productPackagingId.HasValue)
        {
            query = query.Where(s => s.ProductPackagingId == productPackagingId.Value);
        }

        if (!string.IsNullOrWhiteSpace(productName))
        {
            query = query.Where(s => s.ProductPackaging != null &&
                                     s.ProductPackaging.Product != null &&
                                     s.ProductPackaging.Product.Name.Contains(productName));
        }

        // Apply stock status filters
        if (isLowStock && reorderLevel.HasValue)
        {
            query = query.Where(s => s.Batches.Sum(b => b.Quantity - b.ReservedQuantity) > 0 &&
                                     s.Batches.Sum(b => b.Quantity - b.ReservedQuantity) <= reorderLevel.Value);
        }

        if (isOutOfStock)
        {
            query = query.Where(s => !s.Batches.Any() || s.Batches.Sum(b => b.Quantity - b.ReservedQuantity) == 0);
        }

        // Apply ordering
        if (isLowStock)
        {
            query = query
                .OrderBy(s => s.Batches.Sum(b => b.Quantity - b.ReservedQuantity))
                .ThenBy(s => s.ProductPackagingId);
        }
        else
        {
            query = query.OrderBy(s => s.ProductPackagingId);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip(pagingParams.Skip)
            .Take(pagingParams.Take)
            .ToListAsync(cancellationToken);

        return new PagedResult<Stock>(items, totalCount, pagingParams.PageNumber, pagingParams.PageSize);
    }

    public async Task<List<Stock>> GetByPackagingIdsAsync(
        Guid organizationId,
        List<Guid> packagingIds,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(s => s.Batches)
            .Where(s => s.OrganizationId == organizationId && packagingIds.Contains(s.ProductPackagingId))
            .ToListAsync(cancellationToken);
    }

    public async Task<PagedResult<StockBatch>> GetExpiredBatchesAsync(
        Guid organizationId,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var query = _context.Set<StockBatch>()
            .Include(b => b.Stock)
            .Where(b => b.Stock!.OrganizationId == organizationId)
            .Where(b => b.ExpiryDate.HasValue && b.ExpiryDate.Value < now)
            .OrderBy(b => b.ExpiryDate)
            .ThenBy(b => b.BatchNumber);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip(pagingParams.Skip)
            .Take(pagingParams.Take)
            .ToListAsync(cancellationToken);

        return new PagedResult<StockBatch>(items, totalCount, pagingParams.PageNumber, pagingParams.PageSize);
    }

    public async Task<PagedResult<StockBatch>> GetNearExpiryBatchesAsync(
        Guid organizationId,
        int daysThreshold,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var thresholdDate = now.AddDays(daysThreshold);
        var query = _context.Set<StockBatch>()
            .Include(b => b.Stock)
            .Where(b => b.Stock!.OrganizationId == organizationId)
            .Where(b => b.ExpiryDate.HasValue && b.ExpiryDate.Value >= now && b.ExpiryDate.Value <= thresholdDate)
            .OrderBy(b => b.ExpiryDate)
            .ThenBy(b => b.BatchNumber);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip(pagingParams.Skip)
            .Take(pagingParams.Take)
            .ToListAsync(cancellationToken);

        return new PagedResult<StockBatch>(items, totalCount, pagingParams.PageNumber, pagingParams.PageSize);
    }

    public async Task<PagedResult<StockBatch>> GetBatchesByConditionAsync(
        Guid organizationId,
        StockCondition condition,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Set<StockBatch>()
            .Include(b => b.Stock)
            .Where(b => b.Stock!.OrganizationId == organizationId)
            .Where(b => b.Condition == condition)
            .OrderBy(b => b.BatchNumber);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip(pagingParams.Skip)
            .Take(pagingParams.Take)
            .ToListAsync(cancellationToken);

        return new PagedResult<StockBatch>(items, totalCount, pagingParams.PageNumber, pagingParams.PageSize);
    }

    public async Task<StockBatch?> GetBatchByNumberAsync(
        Guid stockId,
        string batchNumber,
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<StockBatch>()
            .FirstOrDefaultAsync(b => b.StockId == stockId && b.BatchNumber == batchNumber, cancellationToken);
    }
}
