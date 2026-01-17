using Domains.Shared.Base;

namespace Domains.Stocks.Entities;

/// <summary>
/// Represents inventory stock for a product packaging at a specific location
/// This is the aggregate root that contains multiple batches
/// </summary>
public class Stock : BaseEntity
{
    private readonly List<StockBatch> _batches = new();

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
        InsertDate = DateTime.UtcNow;
    }

    // Properties
    public Guid ProductPackagingId { get; private set; }
    public Guid OrganizationId { get; private set; }
    public Guid InventoryId { get; private set; }
    public DateTime? LastStocktakeDate { get; private set; }

    // Navigation - Batches collection
    public IReadOnlyCollection<StockBatch> Batches => _batches.AsReadOnly();

    // Computed properties (aggregated from batches)
    public int TotalQuantity => _batches.Sum(b => b.Quantity);
    public int TotalReservedQuantity => _batches.Sum(b => b.ReservedQuantity);
    public int TotalAvailableQuantity => _batches.Sum(b => b.AvailableQuantity);

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

    public void SetLastStocktakeDate(DateTime date)
    {
        LastStocktakeDate = date;
    }

    // Batch Management
    public StockBatch AddBatch(
        string batchNumber,
        int quantity,
        DateTime? expiryDate = null,
        Enums.StockCondition condition = Enums.StockCondition.New,
        Shared.ValueObjects.Price? costPrice = null)
    {
        var batch = StockBatch.Create(
            stockId: Id,
            batchNumber: batchNumber,
            quantity: quantity,
            expiryDate: expiryDate,
            condition: condition,
            costPrice: costPrice
        );

        _batches.Add(batch);
        return batch;
    }

    public void RemoveBatch(Guid batchId)
    {
        var batch = _batches.FirstOrDefault(b => b.Id == batchId);
        if (batch == null)
            throw new InvalidOperationException("الدفعة غير موجودة");

        if (batch.ReservedQuantity > 0)
            throw new InvalidOperationException("لا يمكن حذف دفعة تحتوي على كميات محجوزة");

        _batches.Remove(batch);
    }

    public StockBatch? GetBatch(Guid batchId)
    {
        return _batches.FirstOrDefault(b => b.Id == batchId);
    }

    public StockBatch? GetBatchByNumber(string batchNumber)
    {
        return _batches.FirstOrDefault(b => b.BatchNumber == batchNumber);
    }

    // Query Methods
    public bool IsOutOfStock() => TotalAvailableQuantity == 0;

    public bool IsLowStock(int reorderLevel) => TotalAvailableQuantity > 0 && TotalAvailableQuantity <= reorderLevel;

    public bool HasExpiredBatches() => _batches.Any(b => b.IsExpired());

    public bool HasNearExpiryBatches(int daysThreshold) => _batches.Any(b => b.IsNearExpiry(daysThreshold));

    public IEnumerable<StockBatch> GetExpiredBatches() => _batches.Where(b => b.IsExpired());

    public IEnumerable<StockBatch> GetNearExpiryBatches(int daysThreshold) =>
        _batches.Where(b => b.IsNearExpiry(daysThreshold));

    public IEnumerable<StockBatch> GetAvailableBatches() =>
        _batches.Where(b => b.AvailableQuantity > 0 && !b.IsExpired())
                .OrderBy(b => b.ExpiryDate ?? DateTime.MaxValue); // FEFO: First Expired, First Out
}
