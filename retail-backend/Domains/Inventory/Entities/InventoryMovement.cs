using Domains.Products.Entities;
using Domains.Inventory.Enums;
using Domains.Shared.Base;
using InventoryEntity = Domains.Inventory.Entities.Inventory;

namespace Domains.Inventory.Entities;

/// <summary>
/// Records all inventory movements for audit trail and traceability
/// </summary>
public class InventoryMovement : BaseEntity
{
    // Parameterless constructor for EF Core
    private InventoryMovement()
    {
        Reason = string.Empty;
    }

    // Private constructor to enforce factory methods
    private InventoryMovement(
        Guid productPackagingId,
        int quantity,
        int balanceAfter,
        InventoryMovementType type,
        Guid userId,
        Guid organizationId,
        Guid? inventoryId = null,
        Guid? inventoryOperationItemId = null,
        string? reason = null,
        string? referenceNumber = null)
    {
        Id = Guid.NewGuid();
        ProductPackagingId = productPackagingId;
        Quantity = quantity;
        BalanceAfter = balanceAfter;
        Type = type;
        UserId = userId;
        OrganizationId = organizationId;
        InventoryId = inventoryId;
        InventoryOperationItemId = inventoryOperationItemId;
        Reason = reason ?? string.Empty;
        ReferenceNumber = referenceNumber;
        MovementDate = DateTime.UtcNow;
        InsertDate = DateTime.UtcNow;
    }

    // Properties
    public Guid ProductPackagingId { get; private set; }
    public int Quantity { get; private set; }              // Positive or negative
    public int BalanceAfter { get; private set; }          // Stock balance after movement
    public InventoryMovementType Type { get; private set; }
    public string Reason { get; private set; }
    public string? ReferenceNumber { get; private set; }   // PO#, Sale#, Transfer#, etc.
    public DateTime MovementDate { get; private set; }
    public Guid UserId { get; private set; }
    public Guid? InventoryId { get; private set; }
    public Guid? InventoryOperationItemId { get; private set; }  // Link to bulk operation item
    public Guid OrganizationId { get; private set; }

    // Navigation properties
    public ProductPackaging? Packaging { get; private set; }
    public Inventory? Inventory { get; private set; }
    public InventoryOperationItem? OperationItem { get; private set; }

    /// <summary>
    /// Factory method to create an inventory movement record
    /// </summary>
    public static InventoryMovement Create(
        Guid productPackagingId,
        int quantity,
        int balanceAfter,
        InventoryMovementType type,
        Guid userId,
        Guid organizationId,
        Guid? inventoryId = null,
        Guid? inventoryOperationItemId = null,
        string? reason = null,
        string? referenceNumber = null)
    {
        if (productPackagingId == Guid.Empty)
            throw new ArgumentException("معرف العبوة مطلوب", nameof(productPackagingId));

        if (quantity == 0)
            throw new ArgumentException("الكمية لا يمكن أن تكون صفر", nameof(quantity));

        if (balanceAfter < 0)
            throw new ArgumentException("الرصيد لا يمكن أن يكون سالب", nameof(balanceAfter));

        if (userId == Guid.Empty)
            throw new ArgumentException("معرف المستخدم مطلوب", nameof(userId));

        if (organizationId == Guid.Empty)
            throw new ArgumentException("معرف المؤسسة مطلوب", nameof(organizationId));

        return new InventoryMovement(
            productPackagingId,
            quantity,
            balanceAfter,
            type,
            userId,
            organizationId,
            inventoryId,
            inventoryOperationItemId,
            reason,
            referenceNumber
        );
    }
}
