using Domains.Products.Entities;
using Domains.Shared.Base;
using Domains.Shared.ValueObjects;

namespace Domains.Inventory.Entities;

/// <summary>
/// Represents a single product line item in an inventory operation
/// </summary>
public class InventoryOperationItem : BaseEntity
{
    // Parameterless constructor for EF Core
    private InventoryOperationItem()
    {
        ProductName = string.Empty;
        UnitPrice = null!;
    }

    // Private constructor to enforce factory methods
    private InventoryOperationItem(
        Guid inventoryOperationId,
        Guid productPackagingId,
        string productName,
        string? barcode,
        int quantity,
        Price unitPrice,
        string? notes = null)
    {
        Id = Guid.NewGuid();
        InventoryOperationId = inventoryOperationId;
        ProductPackagingId = productPackagingId;
        ProductName = productName;
        Barcode = barcode;
        Quantity = quantity;
        UnitPrice = unitPrice;
        Notes = notes;
        InsertDate = DateTime.UtcNow;
    }

    // Properties
    public Guid InventoryOperationId { get; private set; }
    public Guid ProductPackagingId { get; private set; }
    public string ProductName { get; private set; }
    public string? Barcode { get; private set; }
    public int Quantity { get; private set; }
    public Price UnitPrice { get; private set; }
    public string? Notes { get; private set; }

    // Navigation properties
    public InventoryOperation? Operation { get; private set; }

    /// <summary>
    /// Factory method to create a new inventory operation item
    /// </summary>
    public static InventoryOperationItem Create(
        Guid inventoryOperationId,
        Guid productPackagingId,
        string productName,
        string? barcode,
        int quantity,
        Price unitPrice,
        string? notes = null)
    {
        if (inventoryOperationId == Guid.Empty)
            throw new ArgumentException("معرف العملية مطلوب", nameof(inventoryOperationId));

        if (productPackagingId == Guid.Empty)
            throw new ArgumentException("معرف العبوة مطلوب", nameof(productPackagingId));

        if (string.IsNullOrWhiteSpace(productName))
            throw new ArgumentException("اسم المنتج مطلوب", nameof(productName));

        if (quantity == 0)
            throw new ArgumentException("الكمية لا يمكن أن تكون صفر", nameof(quantity));

        return new InventoryOperationItem(
            inventoryOperationId,
            productPackagingId,
            productName,
            barcode,
            quantity,
            unitPrice,
            notes
        );
    }

    // Domain methods
    public void UpdateQuantity(int quantity)
    {
        if (quantity == 0)
            throw new ArgumentException("الكمية لا يمكن أن تكون صفر", nameof(quantity));

        Quantity = quantity;
    }

    public void UpdatePrice(Price unitPrice)
    {
        UnitPrice = unitPrice;
    }

    public void UpdateNotes(string? notes)
    {
        Notes = notes;
    }
}
