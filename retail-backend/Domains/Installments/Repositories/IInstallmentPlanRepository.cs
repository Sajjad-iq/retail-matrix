using Domains.Installments.Entities;
using Domains.Installments.Enums;
using Domains.Shared.Base;

namespace Domains.Installments.Repositories;

/// <summary>
/// Repository interface for InstallmentPlan entity
/// </summary>
public interface IInstallmentPlanRepository : IRepository<InstallmentPlan>
{
    // Single queries
    Task<InstallmentPlan?> GetByPlanNumberAsync(string planNumber, CancellationToken cancellationToken = default);

    Task<InstallmentPlan?> GetBySaleIdAsync(Guid saleId, CancellationToken cancellationToken = default);

    Task<InstallmentPlan?> GetWithPaymentsAsync(Guid id, CancellationToken cancellationToken = default);

    // Paginated queries
    Task<PagedResult<InstallmentPlan>> GetByOrganizationAsync(
        Guid organizationId,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default);

    Task<PagedResult<InstallmentPlan>> GetByStatusAsync(
        Guid organizationId,
        InstallmentPlanStatus status,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default);

    Task<PagedResult<InstallmentPlan>> GetByCustomerNameAsync(
        Guid organizationId,
        string customerName,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default);

    Task<PagedResult<InstallmentPlan>> GetByCustomerPhoneAsync(
        Guid organizationId,
        string customerPhone,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default);

    // Business queries
    Task<decimal> GetTotalOutstandingAmountAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default);

    Task<int> GetActivePlansCountAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default);

    Task<int> GetDefaultedPlansCountAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<InstallmentPlan>> GetActivePlansByCustomerPhoneAsync(
        Guid organizationId,
        string customerPhone,
        CancellationToken cancellationToken = default);
}
