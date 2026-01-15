using Domains.Stock.Enums;
using Domains.Shared.Base;
using Domains.Shared.ValueObjects;

namespace Domains.Stock.Entities;

/// <summary>
/// Represents a batch/lot of stock for FIFO/FEFO tracking
/// </summary>
public class StockBatch : BaseEntity
{
    // Parameterless constructor for EF Core
    private StockBatch()
    {
        BatchNumber = string.Empty;
        PurchasePrice = null!;  // Will be set by EF Core
    }

    // Private constructor to enforce factory methods
    private StockBatch(
        Guid productStockId,
        string batchNumber,
        int quantity,
        StockCondition condition,
        Price purchasePrice,
        DateTime? expirationDate = null)
    {
        Id = Guid.NewGuid();
        ProductStockId = productStockId;
        BatchNumber = batchNumber;
        Quantity = quantity;
        RemainingQuantity = quantity;
        ReservedQuantity = 0;
        Condition = condition;
        ExpirationDate = expirationDate;
        PurchasePrice = purchasePrice;
        InsertDate = DateTime.UtcNow;
    }

    // Properties
    public Guid ProductStockId { get; private set; }
    public string BatchNumber { get; private set; }
    public int Quantity { get; private set; }              // Original quantity
    public int RemainingQuantity { get; private set; }     // Current physical quantity
    public int ReservedQuantity { get; private set; }      // Quantity reserved for orders
    public StockCondition Condition { get; private set; }
    public DateTime? ExpirationDate { get; private set; }
    public Price PurchasePrice { get; private set; }

    // Computed property
    public int AvailableQuantity => RemainingQuantity - ReservedQuantity;

    // Navigation properties
    public ProductStock? ProductStock { get; private set; }

    /// <summary>
    /// Factory method to create a stock batch
    /// </summary>
    public static StockBatch Create(
        Guid productStockId,
        string batchNumber,
        int quantity,
        Price purchasePrice,
        DateTime? expirationDate = null,
        StockCondition condition = StockCondition.Good)
    {
        if (productStockId == Guid.Empty)
            throw new ArgumentException("معرف المخزون مطلوب", nameof(productStockId));

        if (string.IsNullOrWhiteSpace(batchNumber))
            throw new ArgumentException("رقم الدفعة مطلوب", nameof(batchNumber));

        if (quantity <= 0)
            throw new ArgumentException("الكمية يجب أن تكون أكبر من صفر", nameof(quantity));

        if (purchasePrice == null)
            throw new ArgumentException("سعر الشراء مطلوب", nameof(purchasePrice));

        if (expirationDate.HasValue && expirationDate.Value <= DateTime.UtcNow)
            throw new ArgumentException("تاريخ الانتهاء يجب أن يكون في المستقبل", nameof(expirationDate));

        return new StockBatch(
            productStockId,
            batchNumber,
            quantity,
            condition,
            purchasePrice,
            expirationDate
        );
    }

    // Domain Methods
    public void ConsumeQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("الكمية يجب أن تكون أكبر من صفر", nameof(quantity));

        if (quantity > RemainingQuantity)
            throw new InvalidOperationException("الكمية المطلوبة أكبر من المتاح في الدفعة");

        RemainingQuantity -= quantity;
    }

    public void ReserveQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("الكمية المحجوزة يجب أن تكون أكبر من صفر", nameof(quantity));

        var newReservedQuantity = ReservedQuantity + quantity;

        if (newReservedQuantity > RemainingQuantity)
            throw new InvalidOperationException("لا يمكن حجز أكثر من المخزون المتاح في الدفعة");

        ReservedQuantity = newReservedQuantity;
    }

    public void ReleaseReservation(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("الكمية المراد إلغاء حجزها يجب أن تكون أكبر من صفر", nameof(quantity));

        var newReservedQuantity = ReservedQuantity - quantity;

        if (newReservedQuantity < 0)
            throw new InvalidOperationException("لا يمكن إلغاء حجز أكثر من المخزون المحجوز");

        ReservedQuantity = newReservedQuantity;
    }

    public void UpdateCondition(StockCondition newCondition)
    {
        Condition = newCondition;
    }

    public bool IsExpired() =>
        ExpirationDate.HasValue && ExpirationDate.Value <= DateTime.UtcNow;

    public bool IsExpiringSoon(int daysThreshold = 30) =>
        ExpirationDate.HasValue &&
        ExpirationDate.Value <= DateTime.UtcNow.AddDays(daysThreshold) &&
        ExpirationDate.Value > DateTime.UtcNow;

    public bool IsFullyConsumed() => RemainingQuantity == 0;
}
