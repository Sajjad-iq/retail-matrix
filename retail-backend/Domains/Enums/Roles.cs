namespace Domains.Enums;

/// <summary>
/// Defines roles/permissions within the system
/// </summary>
public enum Roles


{

    /// <summary>
    /// System administrator - full access
    /// </summary>
    SuperAdmin,
    /// <summary>
    /// System administrator - limited access
    /// </summary>
    Admin,

    /// <summary>
    /// Organization owner - manages organization
    /// </summary>
    Owner,

    /// <summary>
    /// Coordinator - coordinates operations
    /// </summary>
    Coordinator,

    /// <summary>
    /// Cashier - handles transactions
    /// </summary>
    Cashier,

    /// <summary>
    /// Sales representative
    /// </summary>
    Sales,

    /// <summary>
    /// Inventory manager
    /// </summary>
    InventoryManager,

    /// <summary>
    /// Regular user - basic access
    /// </summary>
    User
}
