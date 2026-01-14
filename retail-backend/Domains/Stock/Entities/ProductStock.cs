using Domains.Products.Entities;
using Domains.Stock.Enums;
using Domains.Shared.Base;

namespace Domains.Stock.Entities;

/// <summary>
/// Represents inventory stock for a product packaging at a specific location
/// </summary>
public class ProductStock : BaseEntity
{
    // Parameterless constructor for EF Core
    private ProductStock()
    {
        Batches = new List<StockBatch>();
    }

    // Private constructor to enforce factory methods
    private ProductStock(
        Guid productPackagingId,
        Guid organizationId,
        Guid? locationId = null)
    {
        Id = Guid.NewGuid();
        ProductPackagingId = productPackagingId;
        LocationId = locationId;
        GoodStock = 0;
        DamagedStock = 0;
        ExpiredStock = 0;
        ReservedStock = 0;
        OrganizationId = organizationId;
        Batches = new List<StockBatch>();
        InsertDate = DateTime.UtcNow;
    }

    // Properties - Stock by condition
    public Guid ProductPackagingId { get; private set; }
    public Guid? LocationId { get; private set; }
    public int GoodStock { get; private set; }
    public int DamagedStock { get; private set; }
    public int ExpiredStock { get; private set; }
    public int ReservedStock { get; private set; }
    public DateTime? LastRestockDate { get; private set; }
    public DateTime? LastStocktakeDate { get; private set; }
    public Guid OrganizationId { get; private set; }

    // Computed properties
    public int TotalStock => GoodStock + DamagedStock + ExpiredStock;
    public int AvailableStock => GoodStock - ReservedStock;

    // Navigation properties
    public ProductPackaging? Packaging { get; private set; }
    public List<StockBatch> Batches { get; private set; }

    /// <summary>
    /// Factory method to create a new product stock
    /// </summary>
    public static ProductStock Create(
        Guid productPackagingId,
        Guid organizationId,
        Guid? locationId = null)
    {
        if (productPackagingId == Guid.Empty)
            throw new ArgumentException("معرف العبوة مطلوب", nameof(productPackagingId));

        if (organizationId == Guid.Empty)
            throw new ArgumentException("معرف المؤسسة مطلوب", nameof(organizationId));

        return new ProductStock(
            productPackagingId: productPackagingId,
            organizationId: organizationId,
            locationId: locationId
        );
    }

    // Domain Methods - Stock Adjustment
    public void AdjustGoodStock(int quantity)
    {
        var newStock = GoodStock + quantity;

        if (newStock < 0)
            throw new InvalidOperationException("لا يمكن أن يكون المخزون سالب");

        if (newStock < ReservedStock)
            throw new InvalidOperationException("لا يمكن أن يكون المخزون أقل من المخزون المحجوز");

        GoodStock = newStock;

        if (quantity > 0)
            LastRestockDate = DateTime.UtcNow;
    }

    public void AdjustDamagedStock(int quantity)
    {
        var newStock = DamagedStock + quantity;

        if (newStock < 0)
            throw new InvalidOperationException("لا يمكن أن يكون المخزون التالف سالب");

        DamagedStock = newStock;
    }

    public void AdjustExpiredStock(int quantity)
    {
        var newStock = ExpiredStock + quantity;

        if (newStock < 0)
            throw new InvalidOperationException("لا يمكن أن يكون المخزون المنتهي سالب");

        ExpiredStock = newStock;
    }

    public void SetGoodStock(int quantity)
    {
        if (quantity < 0)
            throw new ArgumentException("الكمية لا يمكن أن تكون سالبة", nameof(quantity));

        if (quantity < ReservedStock)
            throw new InvalidOperationException("لا يمكن أن يكون المخزون أقل من المخزون المحجوز");

        GoodStock = quantity;
    }

    // Reservation Methods
    public void ReserveStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("الكمية المحجوزة يجب أن تكون أكبر من صفر", nameof(quantity));

        var newReservedStock = ReservedStock + quantity;

        if (newReservedStock > GoodStock)
            throw new InvalidOperationException("لا يمكن حجز أكثر من المخزون المتاح");

        ReservedStock = newReservedStock;
    }

    public void ReleaseReservedStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("الكمية المراد إلغاء حجزها يجب أن تكون أكبر من صفر", nameof(quantity));

        var newReservedStock = ReservedStock - quantity;

        if (newReservedStock < 0)
            throw new InvalidOperationException("لا يمكن إلغاء حجز أكثر من المخزون المحجوز");

        ReservedStock = newReservedStock;
    }

    // Batch Management
    public StockBatch AddBatch(
        string batchNumber,
        int quantity,
        decimal costPrice,
        DateTime? expirationDate = null,
        StockCondition condition = StockCondition.Good)
    {
        var batch = StockBatch.Create(
            productStockId: Id,
            batchNumber: batchNumber,
            quantity: quantity,
            costPrice: costPrice,
            expirationDate: expirationDate,
            condition: condition
        );

        Batches.Add(batch);

        // Update stock based on condition
        switch (condition)
        {
            case StockCondition.Good:
                GoodStock += quantity;
                break;
            case StockCondition.Damaged:
            case StockCondition.Defective:
                DamagedStock += quantity;
                break;
            case StockCondition.Expired:
                ExpiredStock += quantity;
                break;
        }

        LastRestockDate = DateTime.UtcNow;
        return batch;
    }

    public void MoveBatchToCondition(Guid batchId, StockCondition newCondition)
    {
        var batch = Batches.FirstOrDefault(b => b.Id == batchId);
        if (batch == null)
            throw new InvalidOperationException("الدفعة غير موجودة");

        var oldCondition = batch.Condition;
        var quantity = batch.RemainingQuantity;

        // Remove from old condition
        switch (oldCondition)
        {
            case StockCondition.Good:
                GoodStock -= quantity;
                break;
            case StockCondition.Damaged:
            case StockCondition.Defective:
                DamagedStock -= quantity;
                break;
            case StockCondition.Expired:
                ExpiredStock -= quantity;
                break;
        }

        // Add to new condition
        switch (newCondition)
        {
            case StockCondition.Good:
                GoodStock += quantity;
                break;
            case StockCondition.Damaged:
            case StockCondition.Defective:
                DamagedStock += quantity;
                break;
            case StockCondition.Expired:
                ExpiredStock += quantity;
                break;
        }

        batch.UpdateCondition(newCondition);
    }

    // Stocktake
    public void RecordStocktake()
    {
        LastStocktakeDate = DateTime.UtcNow;
    }

    // Query Methods
    public bool IsOutOfStock() => AvailableStock == 0;

    public bool IsLowStock(int reorderLevel) => AvailableStock > 0 && AvailableStock <= reorderLevel;

    public IEnumerable<StockBatch> GetExpiredBatches() =>
        Batches.Where(b => b.IsExpired());

    public IEnumerable<StockBatch> GetExpiringSoonBatches(int daysThreshold = 30) =>
        Batches.Where(b => b.IsExpiringSoon(daysThreshold));
}
