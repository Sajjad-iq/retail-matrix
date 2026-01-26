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

    public async Task<PagedResult<Stock>> GetByOrganizationAsync(
        Guid organizationId,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(s => s.Batches)
            .Include(s => s.ProductPackaging)
            .ThenInclude(p => p.Product)
            .Where(s => s.OrganizationId == organizationId)
            .OrderBy(s => s.ProductPackagingId);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip(pagingParams.Skip)
            .Take(pagingParams.Take)
            .ToListAsync(cancellationToken);

        return new PagedResult<Stock>(items, totalCount, pagingParams.PageNumber, pagingParams.PageSize);
    }

    public async Task<PagedResult<Stock>> GetByInventoryAsync(
        Guid inventoryId,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(s => s.Batches)
            .Include(s => s.ProductPackaging)
            .ThenInclude(p => p.Product)
            .Where(s => s.InventoryId == inventoryId)
            .OrderBy(s => s.ProductPackagingId);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip(pagingParams.Skip)
            .Take(pagingParams.Take)
            .ToListAsync(cancellationToken);

        return new PagedResult<Stock>(items, totalCount, pagingParams.PageNumber, pagingParams.PageSize);
    }

    public async Task<PagedResult<Stock>> GetLowStockItemsAsync(
        Guid organizationId,
        int reorderLevel,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(s => s.Batches)
            .Include(s => s.ProductPackaging)
            .ThenInclude(p => p.Product)
            .Where(s => s.OrganizationId == organizationId)
            .Where(s => s.Batches.Sum(b => b.Quantity - b.ReservedQuantity) > 0 &&
                        s.Batches.Sum(b => b.Quantity - b.ReservedQuantity) <= reorderLevel)
            .OrderBy(s => s.Batches.Sum(b => b.Quantity - b.ReservedQuantity))
            .ThenBy(s => s.ProductPackagingId);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip(pagingParams.Skip)
            .Take(pagingParams.Take)
            .ToListAsync(cancellationToken);

        return new PagedResult<Stock>(items, totalCount, pagingParams.PageNumber, pagingParams.PageSize);
    }

    public async Task<PagedResult<Stock>> GetOutOfStockItemsAsync(
        Guid organizationId,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(s => s.Batches)
            .Include(s => s.ProductPackaging)
            .ThenInclude(p => p.Product)
            .Where(s => s.OrganizationId == organizationId)
            .Where(s => !s.Batches.Any() || s.Batches.Sum(b => b.Quantity - b.ReservedQuantity) == 0)
            .OrderBy(s => s.ProductPackagingId);

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
