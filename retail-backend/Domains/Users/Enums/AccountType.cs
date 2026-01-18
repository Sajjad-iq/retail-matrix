namespace Domains.Users.Enums;

/// <summary>
/// Defines the type of account/user in the system
/// </summary>
public enum AccountType
{
    /// <summary>
    /// Business owner - owns an organization
    /// </summary>
    BusinessOwner,

    /// <summary>
    /// Employee - works for an organization
    /// </summary>
    Employee,

    Admin
}
