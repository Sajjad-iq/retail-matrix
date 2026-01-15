namespace Domains.Stock.Enums;

/// <summary>
/// Types of locations - from building level to storage spots
/// </summary>
public enum LocationType
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
