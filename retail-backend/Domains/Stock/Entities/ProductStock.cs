using Domains.Products.Entities;
using Domains.Stock.Enums;
using Domains.Shared.Base;
using Domains.Shared.ValueObjects;

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
        OrganizationId = organizationId;
        Batches = new List<StockBatch>();
        InsertDate = DateTime.UtcNow;
    }

    // Properties
    public Guid ProductPackagingId { get; private set; }
    public Guid? LocationId { get; private set; }
    public DateTime? LastStocktakeDate { get; private set; }
    public Guid OrganizationId { get; private set; }

    // Computed properties - calculated from batches
    public int GoodStock => Batches
        .Where(b => b.Condition == StockCondition.Good)
        .Sum(b => b.RemainingQuantity);

    public int DamagedStock => Batches
        .Where(b => b.Condition == StockCondition.Damaged || b.Condition == StockCondition.Defective)
        .Sum(b => b.RemainingQuantity);

    public int ExpiredStock => Batches
        .Where(b => b.Condition == StockCondition.Expired)
        .Sum(b => b.RemainingQuantity);

    public int ReservedStock => Batches.Sum(b => b.ReservedQuantity);

    public int TotalStock => GoodStock + DamagedStock + ExpiredStock;
    public int AvailableStock => GoodStock - ReservedStock;

    public DateTime? LastRestockDate => Batches
        .OrderByDescending(b => b.InsertDate)
        .Select(b => (DateTime?)b.InsertDate)
        .FirstOrDefault();

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

    // Stock counts are now computed from batches
    // To adjust stock, use AddBatch() or modify batch quantities directly

    // Reservation Methods - FIFO/FEFO based
    public void ReserveStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("الكمية المحجوزة يجب أن تكون أكبر من صفر", nameof(quantity));

        if (quantity > AvailableStock)
            throw new InvalidOperationException("لا يمكن حجز أكثر من المخزون المتاح");

        var remainingToReserve = quantity;

        // Reserve from batches using FIFO (oldest first)
        var availableBatches = Batches
            .Where(b => b.Condition == StockCondition.Good && b.AvailableQuantity > 0)
            .OrderBy(b => b.InsertDate)  // FIFO: oldest first
            .ToList();

        foreach (var batch in availableBatches)
        {
            if (remainingToReserve == 0) break;

            var quantityToReserve = Math.Min(remainingToReserve, batch.AvailableQuantity);
            batch.ReserveQuantity(quantityToReserve);
            remainingToReserve -= quantityToReserve;
        }

        if (remainingToReserve > 0)
            throw new InvalidOperationException("فشل حجز الكمية المطلوبة");
    }

    public void ReleaseReservedStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("الكمية المراد إلغاء حجزها يجب أن تكون أكبر من صفر", nameof(quantity));

        if (quantity > ReservedStock)
            throw new InvalidOperationException("لا يمكن إلغاء حجز أكثر من المخزون المحجوز");

        var remainingToRelease = quantity;

        // Release from batches using FIFO (oldest first)
        var reservedBatches = Batches
            .Where(b => b.ReservedQuantity > 0)
            .OrderBy(b => b.InsertDate)  // FIFO: oldest first
            .ToList();

        foreach (var batch in reservedBatches)
        {
            if (remainingToRelease == 0) break;

            var quantityToRelease = Math.Min(remainingToRelease, batch.ReservedQuantity);
            batch.ReleaseReservation(quantityToRelease);
            remainingToRelease -= quantityToRelease;
        }

        if (remainingToRelease > 0)
            throw new InvalidOperationException("فشل إلغاء حجز الكمية المطلوبة");
    }

    // Batch Management
    public StockBatch AddBatch(
        string batchNumber,
        int quantity,
        Price purchasePrice,
        DateTime? expirationDate = null,
        StockCondition condition = StockCondition.Good)
    {
        var batch = StockBatch.Create(
            productStockId: Id,
            batchNumber: batchNumber,
            quantity: quantity,
            purchasePrice: purchasePrice,
            expirationDate: expirationDate,
            condition: condition
        );

        Batches.Add(batch);

        // All stock counts are automatically computed from batches
        return batch;
    }

    public void MoveBatchToCondition(Guid batchId, StockCondition newCondition)
    {
        var batch = Batches.FirstOrDefault(b => b.Id == batchId);
        if (batch == null)
            throw new InvalidOperationException("الدفعة غير موجودة");

        batch.UpdateCondition(newCondition);

        // Stock counts (GoodStock, DamagedStock, ExpiredStock) are automatically recomputed
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
