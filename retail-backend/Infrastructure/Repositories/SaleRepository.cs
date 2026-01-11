using Domains.Entities;
using Domains.Enums;
using Domains.Repositories;
using Domains.Shared;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of ISaleRepository
/// </summary>
public class SaleRepository : ISaleRepository
{
    private readonly ApplicationDbContext _context;

    public SaleRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    // Single queries
    public async Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .Include(s => s.Payments)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<Sale?> GetBySaleNumberAsync(string saleNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .Include(s => s.Payments)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.SaleNumber == saleNumber, cancellationToken);
    }

    // Paginated queries
    public async Task<PagedResult<Sale>> GetByOrganizationAsync(
        Guid organizationId,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Sales
            .Include(s => s.Items)
            .Include(s => s.Payments)
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
        var query = _context.Sales
            .Include(s => s.Items)
            .Include(s => s.Payments)
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
        var query = _context.Sales
            .Include(s => s.Items)
            .Include(s => s.Payments)
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
        var query = _context.Sales
            .Include(s => s.Items)
            .Include(s => s.Payments)
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

    // CRUD
    public async Task<Sale> AddAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        await _context.Sales.AddAsync(sale, cancellationToken);
        return sale;
    }

    public async Task<Sale> UpdateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        _context.Sales.Update(sale);
        return await Task.FromResult(sale);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var sale = await _context.Sales.FindAsync(new object[] { id }, cancellationToken);

        if (sale == null)
            return false;

        _context.Sales.Remove(sale);
        return true;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    // Business queries
    public async Task<decimal> GetTotalSalesAmountAsync(
        Guid organizationId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .AsNoTracking()
            .Where(s => s.OrganizationId == organizationId
                     && s.SaleDate >= startDate
                     && s.SaleDate <= endDate
                     && s.Status == SaleStatus.Completed)
            .SumAsync(s => s.GrandTotal.Amount, cancellationToken);
    }
}
