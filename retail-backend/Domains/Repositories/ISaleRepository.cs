using Domains.Entities;
using Domains.Enums;
using Domains.Shared;

namespace Domains.Repositories;

/// <summary>
/// Repository interface for Sale entity
/// </summary>
public interface ISaleRepository
{
    // Single queries
    Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Sale?> GetBySaleNumberAsync(string saleNumber, CancellationToken cancellationToken = default);

    // Paginated queries
    Task<PagedResult<Sale>> GetByOrganizationAsync(
        Guid organizationId,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default);

    Task<PagedResult<Sale>> GetByDateRangeAsync(
        Guid organizationId,
        DateTime startDate,
        DateTime endDate,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default);

    Task<PagedResult<Sale>> GetByStatusAsync(
        Guid organizationId,
        SaleStatus status,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default);

    Task<PagedResult<Sale>> GetBySalesPersonAsync(
        Guid salesPersonId,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default);

    Task<PagedResult<Sale>> GetByCustomerAsync(
        Guid customerId,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default);

    // CRUD
    Task<Sale> AddAsync(Sale sale, CancellationToken cancellationToken = default);
    Task<Sale> UpdateAsync(Sale sale, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    // Business queries
    Task<decimal> GetTotalSalesAmountAsync(
        Guid organizationId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);
}
