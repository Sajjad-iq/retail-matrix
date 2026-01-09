using Domains.Entities;
using Domains.Enums;

namespace Domains.Repositories;

/// <summary>
/// Repository interface for Organization aggregate root
/// Defines persistence operations for Organization entities
/// </summary>
public interface IOrganizationRepository
{
    /// <summary>
    /// Retrieves an organization by its unique identifier
    /// </summary>
    /// <param name="id">The organization's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The organization if found, null otherwise</returns>
    Task<Organization?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

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

    /// <summary>
    /// Adds a new organization to the repository
    /// </summary>
    /// <param name="organization">The organization entity to add</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The added organization</returns>
    Task<Organization> AddAsync(Organization organization, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing organization in the repository
    /// </summary>
    /// <param name="organization">The organization entity to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated organization</returns>
    Task<Organization> UpdateAsync(Organization organization, CancellationToken cancellationToken = default);

    /// <summary>
    /// Soft deletes an organization by marking it as deleted
    /// </summary>
    /// <param name="id">The organization's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if deletion was successful, false otherwise</returns>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves all pending changes to the underlying data store
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Number of entities affected</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
