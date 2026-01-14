using Domains.Products.Entities;
using Domains.Stock.Enums;
using Domains.Shared.Base;

namespace Domains.Stock.Entities;

/// <summary>
/// Records all stock movements for audit trail and traceability
/// </summary>
public class StockMovement : BaseEntity
{
    // Parameterless constructor for EF Core
    private StockMovement()
    {
        Reason = string.Empty;
    }

    // Private constructor to enforce factory methods
    private StockMovement(
        Guid productPackagingId,
        int quantity,
        int balanceAfter,
        StockMovementType type,
        Guid userId,
        Guid organizationId,
        Guid? locationId = null,
        string? reason = null,
        string? referenceNumber = null,
        string? batchNumber = null,
        DateTime? expirationDate = null)
    {
        Id = Guid.NewGuid();
        ProductPackagingId = productPackagingId;
        Quantity = quantity;
        BalanceAfter = balanceAfter;
        Type = type;
        UserId = userId;
        OrganizationId = organizationId;
        LocationId = locationId;
        Reason = reason ?? string.Empty;
        ReferenceNumber = referenceNumber;
        BatchNumber = batchNumber;
        ExpirationDate = expirationDate;
        MovementDate = DateTime.UtcNow;
        InsertDate = DateTime.UtcNow;
    }

    // Properties
    public Guid ProductPackagingId { get; private set; }
    public int Quantity { get; private set; }              // Positive or negative
    public int BalanceAfter { get; private set; }          // Stock balance after movement
    public StockMovementType Type { get; private set; }
    public string Reason { get; private set; }
    public string? ReferenceNumber { get; private set; }   // PO#, Sale#, Transfer#, etc.
    public DateTime MovementDate { get; private set; }
    public Guid UserId { get; private set; }
    public Guid? LocationId { get; private set; }
    public Guid OrganizationId { get; private set; }

    // Optional: Batch/Lot tracking
    public string? BatchNumber { get; private set; }
    public DateTime? ExpirationDate { get; private set; }

    // Navigation properties
    public ProductPackaging? Packaging { get; private set; }

    /// <summary>
    /// Factory method to create a stock movement record
    /// </summary>
    public static StockMovement Create(
        Guid productPackagingId,
        int quantity,
        int balanceAfter,
        StockMovementType type,
        Guid userId,
        Guid organizationId,
        Guid? locationId = null,
        string? reason = null,
        string? referenceNumber = null,
        string? batchNumber = null,
        DateTime? expirationDate = null)
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

        return new StockMovement(
            productPackagingId,
            quantity,
            balanceAfter,
            type,
            userId,
            organizationId,
            locationId,
            reason,
            referenceNumber,
            batchNumber,
            expirationDate
        );
    }
}
