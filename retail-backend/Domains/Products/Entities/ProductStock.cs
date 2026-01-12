using Domains.Shared.Base;

namespace Domains.Products.Entities;

/// <summary>
/// Represents inventory stock for a product packaging at a specific location
/// </summary>
public class ProductStock : BaseEntity
{
    // Parameterless constructor for EF Core
    private ProductStock()
    {
    }

    // Private constructor to enforce factory methods
    private ProductStock(
        Guid productPackagingId,
        Guid organizationId,
        Guid? locationId = null,
        DateTime? expirationDate = null)
    {
        Id = Guid.NewGuid();
        ProductPackagingId = productPackagingId;
        LocationId = locationId;
        CurrentStock = 0;
        ReservedStock = 0;
        ExpirationDate = expirationDate;
        OrganizationId = organizationId;
        InsertDate = DateTime.UtcNow;
    }

    // Properties
    public Guid ProductPackagingId { get; private set; }
    public Guid? LocationId { get; private set; }
    public int CurrentStock { get; private set; }
    public int ReservedStock { get; private set; }
    public DateTime? ExpirationDate { get; private set; }
    public DateTime? LastRestockDate { get; private set; }
    public Guid OrganizationId { get; private set; }

    // Computed property
    public int AvailableStock => CurrentStock - ReservedStock;

    // Navigation properties
    public ProductPackaging? Packaging { get; private set; }

    /// <summary>
    /// Factory method to create a new product stock
    /// </summary>
    public static ProductStock Create(
        Guid productPackagingId,
        Guid organizationId,
        Guid? locationId = null,
        DateTime? expirationDate = null)
    {
        if (productPackagingId == Guid.Empty)
            throw new ArgumentException("معرف العبوة مطلوب", nameof(productPackagingId));

        if (organizationId == Guid.Empty)
            throw new ArgumentException("معرف المؤسسة مطلوب", nameof(organizationId));

        if (expirationDate.HasValue && expirationDate.Value <= DateTime.UtcNow)
            throw new ArgumentException("تاريخ الانتهاء يجب أن يكون في المستقبل", nameof(expirationDate));

        return new ProductStock(
            productPackagingId: productPackagingId,
            organizationId: organizationId,
            locationId: locationId,
            expirationDate: expirationDate
        );
    }

    // Domain Methods
    public void AdjustStock(int quantity)
    {
        var newStock = CurrentStock + quantity;

        if (newStock < 0)
            throw new InvalidOperationException("لا يمكن أن يكون المخزون سالب");

        if (newStock < ReservedStock)
            throw new InvalidOperationException("لا يمكن أن يكون المخزون أقل من المخزون المحجوز");

        CurrentStock = newStock;

        if (quantity > 0)
        {
            LastRestockDate = DateTime.UtcNow;
        }

    }

    public void SetStock(int quantity)
    {
        if (quantity < 0)
            throw new ArgumentException("الكمية لا يمكن أن تكون سالبة", nameof(quantity));

        if (quantity < ReservedStock)
            throw new InvalidOperationException("لا يمكن أن يكون المخزون أقل من المخزون المحجوز");

        CurrentStock = quantity;
    }

    public void ReserveStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("الكمية المحجوزة يجب أن تكون أكبر من صفر", nameof(quantity));

        var newReservedStock = ReservedStock + quantity;

        if (newReservedStock > CurrentStock)
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

    public void UpdateExpirationDate(DateTime? expirationDate)
    {
        if (expirationDate.HasValue && expirationDate.Value <= DateTime.UtcNow)
            throw new ArgumentException("تاريخ الانتهاء يجب أن يكون في المستقبل", nameof(expirationDate));

        ExpirationDate = expirationDate;
    }

    public bool IsExpired() => ExpirationDate.HasValue && ExpirationDate.Value <= DateTime.UtcNow;

    public bool IsExpiringSoon(int daysThreshold = 30) =>
        ExpirationDate.HasValue &&
        ExpirationDate.Value <= DateTime.UtcNow.AddDays(daysThreshold) &&
        ExpirationDate.Value > DateTime.UtcNow;

    public bool IsOutOfStock() => AvailableStock == 0;

    public bool IsLowStock(int reorderLevel) => AvailableStock > 0 && AvailableStock <= reorderLevel;
}
