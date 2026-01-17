using Domains.Shared.Base;

namespace Domains.Inventory.Entities;

/// <summary>
/// Represents inventory stock for a product packaging at a specific location
/// </summary>
public class Stock : BaseEntity
{
    // Parameterless constructor for EF Core
    private Stock()
    {
    }

    // Private constructor to enforce factory methods
    private Stock(
        Guid productPackagingId,
        Guid organizationId,
        Guid inventoryId)
    {
        Id = Guid.NewGuid();
        ProductPackagingId = productPackagingId;
        OrganizationId = organizationId;
        InventoryId = inventoryId;
        Quantity = 0;
        ReservedQuantity = 0;
        InsertDate = DateTime.UtcNow;
    }

    // Properties
    public Guid ProductPackagingId { get; private set; }
    public Guid OrganizationId { get; private set; }
    public Guid InventoryId { get; private set; }
    public int Quantity { get; private set; }
    public int ReservedQuantity { get; private set; }
    public DateTime? LastStocktakeDate { get; private set; }

    // Computed property
    public int AvailableQuantity => Quantity - ReservedQuantity;

    // Navigation properties
    public Inventory? Inventory { get; private set; }

    /// <summary>
    /// Factory method to create a new stock record
    /// </summary>
    public static Stock Create(
        Guid productPackagingId,
        Guid organizationId,
        Guid inventoryId)
    {
        if (productPackagingId == Guid.Empty)
            throw new ArgumentException("معرف العبوة مطلوب", nameof(productPackagingId));

        if (organizationId == Guid.Empty)
            throw new ArgumentException("معرف المؤسسة مطلوب", nameof(organizationId));

        if (inventoryId == Guid.Empty)
            throw new ArgumentException("معرف المخزن مطلوب", nameof(inventoryId));

        return new Stock(
            productPackagingId: productPackagingId,
            organizationId: organizationId,
            inventoryId: inventoryId
        );
    }

    // Inventory Management
    public void SetInventory(Guid inventoryId)
    {
        if (inventoryId == Guid.Empty)
            throw new ArgumentException("معرف المخزن مطلوب", nameof(inventoryId));

        InventoryId = inventoryId;
    }

    // Stock Management Methods
    public void AddStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("الكمية يجب أن تكون أكبر من صفر", nameof(quantity));

        Quantity += quantity;
    }

    public void RemoveStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("الكمية يجب أن تكون أكبر من صفر", nameof(quantity));

        if (quantity > AvailableQuantity)
            throw new InvalidOperationException("الكمية المطلوبة أكبر من المخزون المتاح");

        Quantity -= quantity;
    }

    public void SetStock(int quantity)
    {
        if (quantity < 0)
            throw new ArgumentException("الكمية لا يمكن أن تكون سالبة", nameof(quantity));

        if (quantity < ReservedQuantity)
            throw new InvalidOperationException("لا يمكن تعيين الكمية أقل من المحجوز");

        Quantity = quantity;
    }

    // Reservation Methods
    public void Reserve(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("الكمية المحجوزة يجب أن تكون أكبر من صفر", nameof(quantity));

        if (quantity > AvailableQuantity)
            throw new InvalidOperationException("لا يمكن حجز أكثر من المخزون المتاح");

        ReservedQuantity += quantity;
    }

    public void ReleaseReservation(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("الكمية المراد إلغاء حجزها يجب أن تكون أكبر من صفر", nameof(quantity));

        if (quantity > ReservedQuantity)
            throw new InvalidOperationException("لا يمكن إلغاء حجز أكثر من المحجوز");

        ReservedQuantity -= quantity;
    }


    // Query Methods
    public bool IsOutOfStock() => AvailableQuantity == 0;

    public bool IsLowStock(int reorderLevel) => AvailableQuantity > 0 && AvailableQuantity <= reorderLevel;
}
