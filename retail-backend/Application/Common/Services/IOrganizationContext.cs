namespace Application.Common.Services;

/// <summary>
/// Provides access to the current organization context from the C-Organization header.
/// Used by handlers to access the validated organization ID.
/// </summary>
public interface IOrganizationContext
{
    /// <summary>
    /// Gets the current organization ID from the C-Organization header.
    /// This value is validated and the user has been authorized to access this organization.
    /// </summary>
    Guid OrganizationId { get; }

    /// <summary>
    /// Gets the current user ID from the JWT token.
    /// </summary>
    Guid UserId { get; }

    /// <summary>
    /// Gets whether the current user is the owner of the current organization.
    /// </summary>
    bool IsOrganizationOwner { get; }

    /// <summary>
    /// Gets whether the current user is an employee of the current organization.
    /// </summary>
    bool IsOrganizationEmployee { get; }
}
