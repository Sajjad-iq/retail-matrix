using Domains.Stocks.Enums;
using Domains.Shared.Base;

namespace Domains.Stocks.Entities;

/// <summary>
/// Represents a batch of stock with specific expiry date and condition
/// رقعة مخزنية - دفعة من المنتج بتاريخ صلاحية وحالة محددة
/// </summary>
public class StockBatch : BaseEntity
{
    private StockBatch()
    {
    }

    private StockBatch(
        Guid stockId,
        string batchNumber,
        int quantity,
        DateTime? expiryDate,
        StockCondition condition,
        decimal? costPrice)
    {
        Id = Guid.NewGuid();
        StockId = stockId;
        BatchNumber = batchNumber;
        Quantity = quantity;
        ReservedQuantity = 0;
        ExpiryDate = expiryDate;
        Condition = condition;
        CostPrice = costPrice;
        InsertDate = DateTime.UtcNow;
    }

    // Properties
    public Guid StockId { get; private set; }
    public string BatchNumber { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public int ReservedQuantity { get; private set; }
    public DateTime? ExpiryDate { get; private set; }
    public StockCondition Condition { get; private set; }
    public decimal? CostPrice { get; private set; }

    // Computed property
    public int AvailableQuantity => Quantity - ReservedQuantity;

    // Navigation
    public Stock? Stock { get; private set; }

    /// <summary>
    /// Factory method to create a new batch
    /// </summary>
    public static StockBatch Create(
        Guid stockId,
        string batchNumber,
        int quantity,
        DateTime? expiryDate = null,
        StockCondition condition = StockCondition.New,
        decimal? costPrice = null)
    {
        if (stockId == Guid.Empty)
            throw new ArgumentException("معرف المخزون مطلوب", nameof(stockId));

        if (string.IsNullOrWhiteSpace(batchNumber))
            throw new ArgumentException("رقم الدفعة مطلوب", nameof(batchNumber));

        if (quantity < 0)
            throw new ArgumentException("الكمية لا يمكن أن تكون سالبة", nameof(quantity));

        return new StockBatch(
            stockId: stockId,
            batchNumber: batchNumber,
            quantity: quantity,
            expiryDate: expiryDate,
            condition: condition,
            costPrice: costPrice
        );
    }

    // Stock Management
    public void AddQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("الكمية يجب أن تكون أكبر من صفر", nameof(quantity));

        Quantity += quantity;
    }

    public void RemoveQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("الكمية يجب أن تكون أكبر من صفر", nameof(quantity));

        if (quantity > AvailableQuantity)
            throw new InvalidOperationException("الكمية المطلوبة أكبر من الكمية المتاحة في الدفعة");

        Quantity -= quantity;
    }

    public void SetQuantity(int quantity)
    {
        if (quantity < 0)
            throw new ArgumentException("الكمية لا يمكن أن تكون سالبة", nameof(quantity));

        if (quantity < ReservedQuantity)
            throw new InvalidOperationException("لا يمكن تعيين الكمية أقل من المحجوز");

        Quantity = quantity;
    }

    // Reservation
    public void Reserve(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("الكمية المحجوزة يجب أن تكون أكبر من صفر", nameof(quantity));

        if (quantity > AvailableQuantity)
            throw new InvalidOperationException("لا يمكن حجز أكثر من الكمية المتاحة في الدفعة");

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

    // Update Methods
    public void SetCondition(StockCondition condition)
    {
        Condition = condition;
    }

    public void SetExpiryDate(DateTime? expiryDate)
    {
        ExpiryDate = expiryDate;
    }

    public void SetCostPrice(decimal? costPrice)
    {
        CostPrice = costPrice;
    }

    // Query Methods
    public bool IsEmpty() => AvailableQuantity == 0;

    public bool IsExpired() => ExpiryDate.HasValue && ExpiryDate.Value < DateTime.UtcNow;

    public bool IsNearExpiry(int daysThreshold) =>
        ExpiryDate.HasValue &&
        ExpiryDate.Value >= DateTime.UtcNow &&
        ExpiryDate.Value <= DateTime.UtcNow.AddDays(daysThreshold);
}
