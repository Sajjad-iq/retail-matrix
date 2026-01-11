using Domains.Entities;
using Domains.Enums;

namespace Domains.Repositories;

/// <summary>
/// Repository interface for User aggregate root
/// Defines persistence operations for User entities
/// </summary>
public interface IUserRepository : IRepository<User>
{
    /// <summary>
    /// Retrieves a user by their email address
    /// </summary>
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a user by their phone number
    /// </summary>
    Task<User?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all users belonging to a specific organization
    /// </summary>
    Task<List<User>> GetByOrganizationAsync(string organizationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all users with a specific role
    /// </summary>
    Task<List<User>> GetByRoleAsync(Roles role, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all active users
    /// </summary>
    Task<List<User>> GetAllActiveAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a user with the given email already exists
    /// </summary>
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a user with the given phone number already exists
    /// </summary>
    Task<bool> ExistsByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default);
}
