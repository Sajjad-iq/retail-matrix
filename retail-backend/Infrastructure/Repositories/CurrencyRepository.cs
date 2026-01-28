using CurrencyEntity = Domains.Common.Currency.Entities.Currency;
using Domains.Common.Currency.Repositories;
using Domains.Shared.Base;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CurrencyRepository : Repository<CurrencyEntity>, ICurrencyRepository
{
    public CurrencyRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<CurrencyEntity?> GetByCodeAsync(string code, Guid organizationId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Code == code && c.OrganizationId == organizationId, cancellationToken);
    }

    public async Task<CurrencyEntity?> GetBaseCurrencyAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.IsBaseCurrency && c.OrganizationId == organizationId, cancellationToken);
    }

    public async Task<IEnumerable<CurrencyEntity>> GetByOrganizationAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(c => c.OrganizationId == organizationId)
            .OrderBy(c => c.Code)
            .ToListAsync(cancellationToken);
    }

    public async Task<PagedResult<CurrencyEntity>> GetByOrganizationAsync(
        Guid organizationId,
        Domains.Common.Currency.Enums.CurrencyStatus? status,
        string? searchTerm,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsNoTracking().Where(c => c.OrganizationId == organizationId);

        // Apply status filter
        if (status.HasValue)
        {
            query = query.Where(c => c.Status == status.Value);
        }

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLower();
            query = query.Where(c =>
                c.Code.ToLower().Contains(term) ||
                c.Name.ToLower().Contains(term) ||
                c.Symbol.ToLower().Contains(term));
        }

        // Order by: Base currency first, then by code
        query = query.OrderByDescending(c => c.IsBaseCurrency).ThenBy(c => c.Code);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip(pagingParams.Skip)
            .Take(pagingParams.Take)
            .ToListAsync(cancellationToken);

        return new PagedResult<CurrencyEntity>(items, totalCount, pagingParams.PageNumber, pagingParams.PageSize);
    }

    public async Task<IEnumerable<CurrencyEntity>> GetActiveAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(c => c.OrganizationId == organizationId && c.Status == Domains.Common.Currency.Enums.CurrencyStatus.Active)
            .OrderBy(c => c.Code)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByCodeAsync(string code, Guid organizationId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .AnyAsync(c => c.Code == code && c.OrganizationId == organizationId, cancellationToken);
    }

    public async Task<bool> HasBaseCurrencyAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .AnyAsync(c => c.IsBaseCurrency && c.OrganizationId == organizationId, cancellationToken);
    }
}
