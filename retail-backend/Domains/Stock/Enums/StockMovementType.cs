namespace Domains.Stock.Enums;

/// <summary>
/// Types of stock movements for inventory tracking
/// </summary>
public enum StockMovementType
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
