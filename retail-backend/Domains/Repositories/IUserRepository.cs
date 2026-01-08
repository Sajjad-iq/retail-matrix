using Domains.Entities;

namespace Domains.Repositories;

/// <summary>
/// Repository interface for User aggregate root
/// Defines persistence operations for User entities
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Retrieves a user by their unique identifier
    /// </summary>
    /// <param name="id">The user's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The user if found, null otherwise</returns>
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a user by their email address
    /// </summary>
    /// <param name="email">The user's email address</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The user if found, null otherwise</returns>
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a user by their phone number
    /// </summary>
    /// <param name="phoneNumber">The user's phone number</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The user if found, null otherwise</returns>
    Task<User?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all users belonging to a specific organization
    /// </summary>
    /// <param name="organizationId">The organization identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of users in the organization</returns>
    Task<List<User>> GetByOrganizationAsync(string organizationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all users with a specific role
    /// </summary>
    /// <param name="role">The role to filter by</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of users with the specified role</returns>
    Task<List<User>> GetByRoleAsync(Roles role, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all active users
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of active users</returns>
    Task<List<User>> GetAllActiveAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a user with the given email already exists
    /// </summary>
    /// <param name="email">The email to check</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if email exists, false otherwise</returns>
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a user with the given phone number already exists
    /// </summary>
    /// <param name="phoneNumber">The phone number to check</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if phone number exists, false otherwise</returns>
    Task<bool> ExistsByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new user to the repository
    /// </summary>
    /// <param name="user">The user entity to add</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The added user</returns>
    Task<User> AddAsync(User user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing user in the repository
    /// </summary>
    /// <param name="user">The user entity to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated user</returns>
    Task<User> UpdateAsync(User user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Soft deletes a user by marking them as deleted
    /// </summary>
    /// <param name="id">The user's unique identifier</param>
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
