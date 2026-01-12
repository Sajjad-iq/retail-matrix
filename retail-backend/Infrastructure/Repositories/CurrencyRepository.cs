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
