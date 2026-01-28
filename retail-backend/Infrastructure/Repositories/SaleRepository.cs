using Domains.Sales.Entities;
using Domains.Sales.Repositories;
using Domains.Sales.Enums;
using Domains.Shared.Base;
using Domains.Products.Enums;
using Domains.Organizations.Enums;
using Domains.Users.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

/// <summary>
/// Entity Framework Core implementation of ISaleRepository
/// </summary>
public class SaleRepository : Repository<Sale>, ISaleRepository
{
    public SaleRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Sale?> GetBySaleNumberAsync(string saleNumber, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(s => s.Items)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.SaleNumber == saleNumber, cancellationToken);
    }

    public async Task<Sale?> GetByIdWithTrackingAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<Sale?> GetActiveDraftSaleAsync(Guid inventoryId, Guid userId, CancellationToken cancellationToken = default)
    {
        // Note: Sale doesn't store InventoryId, so we just get the most recent draft sale for the user
        return await _dbSet
            .Include(s => s.Items)
            .AsNoTracking()
            .Where(s => s.SalesPersonId == userId && s.Status == SaleStatus.Draft)
            .OrderByDescending(s => s.SaleDate)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Domains.Common.Currency.Entities.Currency?> GetBaseCurrencyAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Domains.Common.Currency.Entities.Currency>()
            .AsNoTracking()
            .FirstOrDefaultAsync(c => 
                c.OrganizationId == organizationId && 
                c.IsBaseCurrency && 
                !c.IsDeleted, 
                cancellationToken);
    }

    public async Task<PagedResult<Sale>> GetByOrganizationAsync(
        Guid organizationId,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(s => s.Items)
            .AsNoTracking()
            .Where(s => s.OrganizationId == organizationId)
            .OrderByDescending(s => s.SaleDate);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip(pagingParams.Skip)
            .Take(pagingParams.Take)
            .ToListAsync(cancellationToken);

        return new PagedResult<Sale>(items, totalCount, pagingParams.PageNumber, pagingParams.PageSize);
    }

    public async Task<PagedResult<Sale>> GetByDateRangeAsync(
        Guid organizationId,
        DateTime startDate,
        DateTime endDate,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(s => s.Items)
            .AsNoTracking()
            .Where(s => s.OrganizationId == organizationId
                     && s.SaleDate >= startDate
                     && s.SaleDate <= endDate)
            .OrderByDescending(s => s.SaleDate);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip(pagingParams.Skip)
            .Take(pagingParams.Take)
            .ToListAsync(cancellationToken);

        return new PagedResult<Sale>(items, totalCount, pagingParams.PageNumber, pagingParams.PageSize);
    }

    public async Task<PagedResult<Sale>> GetByStatusAsync(
        Guid organizationId,
        SaleStatus status,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(s => s.Items)
            .AsNoTracking()
            .Where(s => s.OrganizationId == organizationId && s.Status == status)
            .OrderByDescending(s => s.SaleDate);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip(pagingParams.Skip)
            .Take(pagingParams.Take)
            .ToListAsync(cancellationToken);

        return new PagedResult<Sale>(items, totalCount, pagingParams.PageNumber, pagingParams.PageSize);
    }

    public async Task<PagedResult<Sale>> GetBySalesPersonAsync(
        Guid salesPersonId,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(s => s.Items)
            .AsNoTracking()
            .Where(s => s.SalesPersonId == salesPersonId)
            .OrderByDescending(s => s.SaleDate);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip(pagingParams.Skip)
            .Take(pagingParams.Take)
            .ToListAsync(cancellationToken);

        return new PagedResult<Sale>(items, totalCount, pagingParams.PageNumber, pagingParams.PageSize);
    }

    public async Task<decimal> GetTotalSalesAmountAsync(
        Guid organizationId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(s => s.OrganizationId == organizationId
                     && s.SaleDate >= startDate
                     && s.SaleDate <= endDate
                     && s.Status == SaleStatus.Completed)
            .SumAsync(s => s.GrandTotal.Amount, cancellationToken);
    }

    // Override GetByIdAsync to include related entities
    public override async Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(s => s.Items)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }
}
