using Domains.Sales.Entities;
using Domains.Sales.Enums;
using Domains.Shared.Base;

namespace Domains.Sales.Repositories;

/// <summary>
/// Repository interface for Sale entity
/// </summary>
public interface ISaleRepository : IRepository<Sale>
{
    // Single queries
    Task<Sale?> GetBySaleNumberAsync(string saleNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get sale by ID with tracking (for updates)
    /// </summary>
    Task<Sale?> GetByIdWithTrackingAsync(Guid id, CancellationToken cancellationToken = default);

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

    // Business queries
    Task<decimal> GetTotalSalesAmountAsync(
        Guid organizationId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);
}
