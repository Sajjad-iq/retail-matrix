using Domains.Entities;
using Domains.Enums;

namespace Domains.Repositories;

/// <summary>
/// Repository interface for Organization aggregate root
/// Defines persistence operations for Organization entities
/// </summary>
public interface IOrganizationRepository : IRepository<Organization>
{
    /// <summary>
    /// Retrieves an organization by its domain name
    /// </summary>
    /// <param name="domain">The organization's domain</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The organization if found, null otherwise</returns>
    Task<Organization?> GetByDomainAsync(string domain, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all organizations with a specific status
    /// </summary>
    /// <param name="status">The status to filter by</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of organizations with the specified status</returns>
    Task<List<Organization>> GetByStatusAsync(OrganizationStatus status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all organizations created by a specific user
    /// </summary>
    /// <param name="userId">The user's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of organizations created by the user</returns>
    Task<List<Organization>> GetByCreatorAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all active organizations
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of active organizations</returns>
    Task<List<Organization>> GetAllActiveAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all organizations (including inactive)
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of all organizations</returns>
    Task<List<Organization>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if an organization with the given domain already exists
    /// </summary>
    /// <param name="domain">The domain to check</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if domain exists, false otherwise</returns>
    Task<bool> ExistsByDomainAsync(string domain, CancellationToken cancellationToken = default);
}
