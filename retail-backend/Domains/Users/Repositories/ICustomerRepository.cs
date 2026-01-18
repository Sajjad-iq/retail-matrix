using Domains.Users.Entities;
using Domains.Shared.Base;

namespace Domains.Users.Repositories;

/// <summary>
/// Repository interface for Customer entity
/// </summary>
public interface ICustomerRepository : IRepository<Customer>
{
    /// <summary>
    /// Gets a customer by phone number
    /// </summary>
    Task<Customer?> GetByPhoneNumberAsync(
        Guid organizationId,
        string phoneNumber,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches customers by name or phone number
    /// </summary>
    Task<PagedResult<Customer>> SearchAsync(
        Guid organizationId,
        string searchTerm,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all customers for an organization
    /// </summary>
    Task<PagedResult<Customer>> GetByOrganizationAsync(
        Guid organizationId,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default);
}
