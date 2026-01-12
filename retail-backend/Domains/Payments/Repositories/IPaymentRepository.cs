using Domains.Payments.Entities;
using Domains.Payments.Enums;
using Domains.Shared.Base;

namespace Domains.Payments.Repositories;

/// <summary>
/// Repository interface for Payment entity
/// </summary>
public interface IPaymentRepository : IRepository<Payment>
{
    /// <summary>
    /// Get all payments for a specific entity
    /// </summary>
    Task<IEnumerable<Payment>> GetByEntityIdAsync(
        Guid entityId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all payments for a specific entity and type
    /// </summary>
    Task<IEnumerable<Payment>> GetByEntityAsync(
        Guid entityId,
        PaymentEntityType entityType,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get payments by status
    /// </summary>
    Task<PagedResult<Payment>> GetByStatusAsync(
        PaymentStatus status,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get payments by payment method
    /// </summary>
    Task<PagedResult<Payment>> GetByPaymentMethodAsync(
        PaymentMethod paymentMethod,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get payments within a date range
    /// </summary>
    Task<PagedResult<Payment>> GetByDateRangeAsync(
        DateTime startDate,
        DateTime endDate,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default);
}
