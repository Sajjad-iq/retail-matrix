using Domains.Stocks.Entities;
using Domains.Stocks.Enums;
using Domains.Stocks.Models;
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

    public async Task<PagedResult<Stock>> GetListAsync(
        Guid organizationId,
        StockFilter filter,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(s => s.ProductPackaging)
            .ThenInclude(p => p.Product)
            .Include(s => s.Inventory)
            .Where(s => s.OrganizationId == organizationId);

        // Apply filters
        if (filter.InventoryId.HasValue)
        {
            query = query.Where(s => s.InventoryId == filter.InventoryId.Value);
        }

        if (filter.ProductId.HasValue)
        {
            query = query.Where(s => s.ProductPackaging != null &&
                                     s.ProductPackaging.ProductId == filter.ProductId.Value);
        }

        if (filter.ProductPackagingId.HasValue)
        {
            query = query.Where(s => s.ProductPackagingId == filter.ProductPackagingId.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.ProductName))
        {
            query = query.Where(s => s.ProductPackaging != null &&
                                     s.ProductPackaging.Product != null &&
                                     s.ProductPackaging.Product.Name.Contains(filter.ProductName));
        }

        // Apply stock status filters
        if (filter.Status == StockStatus.LowStock && filter.ReorderLevel.HasValue)
        {
            query = query.Where(s => s.Batches.Sum(b => b.Quantity - b.ReservedQuantity) > 0 &&
                                     s.Batches.Sum(b => b.Quantity - b.ReservedQuantity) <= filter.ReorderLevel.Value);
        }
        else if (filter.Status == StockStatus.OutOfStock)
        {
            query = query.Where(s => !s.Batches.Any() || s.Batches.Sum(b => b.Quantity - b.ReservedQuantity) == 0);
        }

        // Apply ordering
        if (filter.Status == StockStatus.LowStock)
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
            .Include(s => s.Batches)
            .Skip(pagingParams.Skip)
            .Take(pagingParams.Take)
            .AsSplitQuery()
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

    public async Task<PagedResult<StockBatch>> GetBatchesListAsync(
        Guid organizationId,
        StockBatchFilter filter,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Set<StockBatch>()
            .Include(b => b.Stock)
            .Where(b => b.Stock!.OrganizationId == organizationId);

        if (filter.StockId.HasValue)
        {
            query = query.Where(b => b.StockId == filter.StockId.Value);
        }

        if (filter.Condition.HasValue)
        {
            query = query.Where(b => b.Condition == filter.Condition.Value);
        }

        if (filter.IsExpired == true)
        {
            var now = DateTime.UtcNow;
            query = query.Where(b => b.ExpiryDate.HasValue && b.ExpiryDate.Value < now);
        }

        if (filter.DaysToExpiry.HasValue)
        {
            var now = DateTime.UtcNow;
            var thresholdDate = now.AddDays(filter.DaysToExpiry.Value);
            query = query.Where(b => b.ExpiryDate.HasValue && b.ExpiryDate.Value >= now && b.ExpiryDate.Value <= thresholdDate);
        }

        query = query.OrderBy(b => b.ExpiryDate).ThenBy(b => b.BatchNumber);

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
