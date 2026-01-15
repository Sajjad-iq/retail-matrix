namespace Domains.Inventory.Enums;

/// <summary>
/// Status of inventory operation
/// </summary>
public enum InventoryOperationStatus
{
    Draft,          // Being prepared
    Completed,      // Finalized and applied
    Cancelled       // Cancelled/voided
}
