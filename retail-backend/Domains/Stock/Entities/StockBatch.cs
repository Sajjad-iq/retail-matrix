using Domains.Stock.Enums;
using Domains.Shared.Base;

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
    }

    // Private constructor to enforce factory methods
    private StockBatch(
        Guid productStockId,
        string batchNumber,
        int quantity,
        StockCondition condition,
        DateTime receivedDate,
        decimal costPrice,
        DateTime? expirationDate = null)
    {
        Id = Guid.NewGuid();
        ProductStockId = productStockId;
        BatchNumber = batchNumber;
        Quantity = quantity;
        RemainingQuantity = quantity;
        Condition = condition;
        ReceivedDate = receivedDate;
        ExpirationDate = expirationDate;
        CostPrice = costPrice;
        InsertDate = DateTime.UtcNow;
    }

    // Properties
    public Guid ProductStockId { get; private set; }
    public string BatchNumber { get; private set; }
    public int Quantity { get; private set; }              // Original quantity
    public int RemainingQuantity { get; private set; }     // Current quantity
    public StockCondition Condition { get; private set; }
    public DateTime ReceivedDate { get; private set; }
    public DateTime? ExpirationDate { get; private set; }
    public decimal CostPrice { get; private set; }

    // Navigation properties
    public ProductStock? ProductStock { get; private set; }

    /// <summary>
    /// Factory method to create a stock batch
    /// </summary>
    public static StockBatch Create(
        Guid productStockId,
        string batchNumber,
        int quantity,
        decimal costPrice,
        DateTime? expirationDate = null,
        StockCondition condition = StockCondition.Good)
    {
        if (productStockId == Guid.Empty)
            throw new ArgumentException("معرف المخزون مطلوب", nameof(productStockId));

        if (string.IsNullOrWhiteSpace(batchNumber))
            throw new ArgumentException("رقم الدفعة مطلوب", nameof(batchNumber));

        if (quantity <= 0)
            throw new ArgumentException("الكمية يجب أن تكون أكبر من صفر", nameof(quantity));

        if (costPrice < 0)
            throw new ArgumentException("سعر التكلفة لا يمكن أن يكون سالب", nameof(costPrice));

        if (expirationDate.HasValue && expirationDate.Value <= DateTime.UtcNow)
            throw new ArgumentException("تاريخ الانتهاء يجب أن يكون في المستقبل", nameof(expirationDate));

        return new StockBatch(
            productStockId,
            batchNumber,
            quantity,
            condition,
            DateTime.UtcNow,
            costPrice,
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
