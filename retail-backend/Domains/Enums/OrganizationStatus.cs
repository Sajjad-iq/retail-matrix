namespace Domains.Enums;

/// <summary>
/// Represents the operational status of an organization
/// </summary>
public enum OrganizationStatus
{
    /// <summary>
    /// Organization is active and operational
    /// </summary>
    Active,

    /// <summary>
    /// Organization is temporarily suspended
    /// </summary>
    Suspended,

    /// <summary>
    /// Organization is pending approval/activation
    /// </summary>
    Pending,

    /// <summary>
    /// Organization is permanently closed
    /// </summary>
    Closed
}
