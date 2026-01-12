using Domains.Payments.Entities;
using Domains.Payments.Enums;
using Domains.Payments.Repositories;
using Domains.Shared.Base;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Payment entity
/// </summary>
public class PaymentRepository : Repository<Payment>, IPaymentRepository
{
    public PaymentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Payment>> GetByEntityIdAsync(Guid entityId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(p => p.EntityId == entityId)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Payment>> GetByEntityAsync(
        Guid entityId,
        PaymentEntityType entityType,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(p => p.EntityId == entityId && p.EntityType == entityType)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<PagedResult<Payment>> GetByStatusAsync(
        PaymentStatus status,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .AsNoTracking()
            .Where(p => p.Status == status)
            .OrderByDescending(p => p.PaymentDate);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip(pagingParams.Skip)
            .Take(pagingParams.Take)
            .ToListAsync(cancellationToken);

        return new PagedResult<Payment>(items, totalCount, pagingParams.PageNumber, pagingParams.PageSize);
    }

    public async Task<PagedResult<Payment>> GetByPaymentMethodAsync(
        PaymentMethod paymentMethod,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .AsNoTracking()
            .Where(p => p.PaymentMethod == paymentMethod)
            .OrderByDescending(p => p.PaymentDate);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip(pagingParams.Skip)
            .Take(pagingParams.Take)
            .ToListAsync(cancellationToken);

        return new PagedResult<Payment>(items, totalCount, pagingParams.PageNumber, pagingParams.PageSize);
    }

    public async Task<PagedResult<Payment>> GetByDateRangeAsync(
        DateTime startDate,
        DateTime endDate,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .AsNoTracking()
            .Where(p => p.PaymentDate >= startDate && p.PaymentDate <= endDate)
            .OrderByDescending(p => p.PaymentDate);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip(pagingParams.Skip)
            .Take(pagingParams.Take)
            .ToListAsync(cancellationToken);

        return new PagedResult<Payment>(items, totalCount, pagingParams.PageNumber, pagingParams.PageSize);
    }
}
