namespace Domains.Inventory.Enums;

/// <summary>
/// Types of inventory operations
/// </summary>
public enum InventoryOperationType
{
    Purchase,       // Receiving from supplier
    Sale,           // Selling to customer
    Transfer,       // Moving between locations
    Stocktake,      // Physical count adjustment
    Adjustment,     // Manual adjustment
    Return,         // Return from customer
    Damage,         // Damaged items
    Expired         // Expired items
}
