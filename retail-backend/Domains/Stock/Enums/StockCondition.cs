namespace Domains.Stock.Enums;

/// <summary>
/// Condition/status of stock items
/// </summary>
public enum StockCondition
{
    Good,           // Normal, sellable condition
    Damaged,        // Damaged but might be repairable
    Defective,      // Manufacturing defect
    Expired,        // Past expiration date
    NearExpiry,     // Expiring soon (within threshold)
    Quarantine,     // Under inspection/quality check
    Reserved,       // Reserved for specific order
    Obsolete        // No longer sold/discontinued
}
