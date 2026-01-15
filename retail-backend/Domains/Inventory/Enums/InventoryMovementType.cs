namespace Domains.Inventory.Enums;

/// <summary>
/// Types of inventory movements for tracking
/// </summary>
public enum InventoryMovementType
{
    // Inbound (increase stock)
    Purchase,           // Stock received from supplier
    Adjustment,         // Manual adjustment (increase or decrease)

    // Outbound (decrease stock)
    Sale,               // Sold to customer
    Damage,             // Damaged/broken items
    Expired,            // Expired/near expiry items

    // Special
    Stocktake,          // Physical count adjustment
    Transfer            // Transfer between locations
}
