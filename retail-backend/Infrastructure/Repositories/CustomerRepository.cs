using Domains.Users.Entities;
using Domains.Users.Repositories;
using Domains.Shared.Base;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Customer entity
/// </summary>
public class CustomerRepository : Repository<Customer>, ICustomerRepository
{
    public CustomerRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Customer?> GetByPhoneNumberAsync(
        Guid organizationId,
        string phoneNumber,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(
                c => c.OrganizationId == organizationId && c.PhoneNumber == phoneNumber,
                cancellationToken);
    }

    public async Task<PagedResult<Customer>> SearchAsync(
        Guid organizationId,
        string searchTerm,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Where(c => c.OrganizationId == organizationId)
            .Where(c => c.Name.Contains(searchTerm) || c.PhoneNumber.Contains(searchTerm))
            .OrderBy(c => c.Name);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pagingParams.PageNumber - 1) * pagingParams.PageSize)
            .Take(pagingParams.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Customer>(items, totalCount, pagingParams.PageNumber, pagingParams.PageSize);
    }

    public async Task<PagedResult<Customer>> GetByOrganizationAsync(
        Guid organizationId,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Where(c => c.OrganizationId == organizationId)
            .OrderBy(c => c.Name);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pagingParams.PageNumber - 1) * pagingParams.PageSize)
            .Take(pagingParams.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Customer>(items, totalCount, pagingParams.PageNumber, pagingParams.PageSize);
    }
}
