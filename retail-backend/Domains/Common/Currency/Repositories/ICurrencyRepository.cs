using CurrencyEntity = Domains.Common.Currency.Entities.Currency;
using Domains.Shared.Base;

namespace Domains.Common.Currency.Repositories;

/// <summary>
/// Repository interface for Currency aggregate
/// </summary>
public interface ICurrencyRepository : IRepository<CurrencyEntity>
{
    Task<CurrencyEntity?> GetByCodeAsync(string code, Guid organizationId, CancellationToken cancellationToken = default);
    Task<CurrencyEntity?> GetBaseCurrencyAsync(Guid organizationId, CancellationToken cancellationToken = default);
    Task<IEnumerable<CurrencyEntity>> GetByOrganizationAsync(Guid organizationId, CancellationToken cancellationToken = default);
    Task<IEnumerable<CurrencyEntity>> GetActiveAsync(Guid organizationId, CancellationToken cancellationToken = default);
    Task<bool> ExistsByCodeAsync(string code, Guid organizationId, CancellationToken cancellationToken = default);
    Task<bool> HasBaseCurrencyAsync(Guid organizationId, CancellationToken cancellationToken = default);
}
