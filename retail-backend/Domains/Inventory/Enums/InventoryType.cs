namespace Domains.Inventory.Enums;

/// <summary>
/// Types of inventory locations - from building level to storage spots
/// </summary>
public enum InventoryType
{
    // Building level (المباني)
    Warehouse,      // مستودع
    // Storage spots within building (أماكن التخزين)
    Aisle,          // ممر
    Rack,           // رف كبير
    Shelf,          // رف
    Bin,            // صندوق
    Drawer,         // درج
    Floor           // أرضية
}
