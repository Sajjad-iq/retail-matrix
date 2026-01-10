using Domains.Enums;
using Domains.Shared;
using Domains.ValueObjects;

namespace Domains.Entities;

/// <summary>
/// Represents a packaging variant of a product with pricing
/// </summary>
public class ProductPackaging : BaseEntity
{
    // Parameterless constructor for EF Core
    private ProductPackaging()
    {
        CostPrice = Price.Create(0, "IQD");
        SellingPrice = Price.Create(0, "IQD");
    }

    // Private constructor to enforce factory methods
    private ProductPackaging(
        Guid productId,
        Price costPrice,
        Price sellingPrice,
        UnitOfMeasure unitOfMeasure,
        Guid organizationId,
        string? barcode = null,
        int unitsPerPackage = 1,
        int reorderLevel = 10,
        bool isDefault = false,
        string? imageUrl = null,
        string? dimensions = null)
    {
        Id = Guid.NewGuid();
        ProductId = productId;
        Barcode = barcode;
        UnitsPerPackage = unitsPerPackage;
        UnitOfMeasure = unitOfMeasure;
        CostPrice = costPrice;
        SellingPrice = sellingPrice;
        ReorderLevel = reorderLevel;
        IsDefault = isDefault;
        IsActive = true;
        ImageUrl = imageUrl;
        Dimensions = dimensions;
        OrganizationId = organizationId;
        InsertDate = DateTime.UtcNow;
    }

    // Properties
    public Guid ProductId { get; private set; }
    public string? Barcode { get; private set; }
    public int UnitsPerPackage { get; private set; }
    public UnitOfMeasure UnitOfMeasure { get; private set; }
    public Price CostPrice { get; private set; }
    public Price SellingPrice { get; private set; }
    public int ReorderLevel { get; private set; }
    public bool IsDefault { get; private set; }
    public bool IsActive { get; private set; }
    public string? ImageUrl { get; private set; }
    public string? Dimensions { get; private set; } // Dimensions as string (e.g., "10x20x30 cm", "5x8x12 in")
    public Guid OrganizationId { get; private set; }

    // Navigation properties
    public Product? Product { get; private set; }

    /// <summary>
    /// Factory method to create a new product packaging
    /// </summary>
    public static ProductPackaging Create(
        Guid productId,
        Price costPrice,
        Price sellingPrice,
        UnitOfMeasure unitOfMeasure,
        Guid organizationId,
        string? barcode = null,
        int unitsPerPackage = 1,
        int reorderLevel = 10,
        bool isDefault = false,
        string? imageUrl = null,
        string? dimensions = null)
    {

        if (sellingPrice.Amount < costPrice.Amount)
            throw new ArgumentException("سعر البيع يجب أن يكون أكبر من أو يساوي سعر التكلفة", nameof(sellingPrice));

        // Validate Barcode if provided
        if (barcode != null)
        {
            _ = ValueObjects.Barcode.Create(barcode); // Validate only
        }

        if (productId == Guid.Empty)
            throw new ArgumentException("معرف المنتج مطلوب", nameof(productId));

        if (organizationId == Guid.Empty)
            throw new ArgumentException("معرف المؤسسة مطلوب", nameof(organizationId));

        if (unitsPerPackage <= 0)
            throw new ArgumentException("عدد الوحدات في العبوة يجب أن يكون أكبر من صفر", nameof(unitsPerPackage));

        if (reorderLevel < 0)
            throw new ArgumentException("مستوى إعادة الطلب يجب أن يكون صفر أو أكثر", nameof(reorderLevel));

        return new ProductPackaging(
            productId: productId,
            costPrice: costPrice,
            sellingPrice: sellingPrice,
            unitOfMeasure: unitOfMeasure,
            organizationId: organizationId,
            barcode: barcode,
            unitsPerPackage: unitsPerPackage,
            reorderLevel: reorderLevel,
            isDefault: isDefault,
            imageUrl: imageUrl,
            dimensions: dimensions
        );
    }

    // Domain Methods
    public void UpdatePricing(Price costPrice, Price sellingPrice)
    {
        if (sellingPrice.Amount < costPrice.Amount)
            throw new ArgumentException("سعر البيع يجب أن يكون أكبر من أو يساوي سعر التكلفة", nameof(sellingPrice));

        CostPrice = costPrice;
        SellingPrice = sellingPrice;
    }

    public void UpdateBarcode(string barcode)
    {
        var productBarcode = ValueObjects.Barcode.Create(barcode);
        Barcode = productBarcode;
    }

    public void UpdatePackagingInfo(int unitsPerPackage, UnitOfMeasure unitOfMeasure)
    {
        if (unitsPerPackage <= 0)
            throw new ArgumentException("عدد الوحدات في العبوة يجب أن يكون أكبر من صفر", nameof(unitsPerPackage));

        UnitsPerPackage = unitsPerPackage;
        UnitOfMeasure = unitOfMeasure;
    }

    public void UpdateReorderLevel(int reorderLevel)
    {
        if (reorderLevel < 0)
            throw new ArgumentException("مستوى إعادة الطلب يجب أن يكون صفر أو أكثر", nameof(reorderLevel));

        ReorderLevel = reorderLevel;
    }

    public void SetAsDefault()
    {
        IsDefault = true;
    }

    public void UnsetDefault()
    {
        IsDefault = false;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void UpdateImage(string? imageUrl)
    {
        ImageUrl = imageUrl;
    }

    public void UpdateDimensions(string? dimensions)
    {
        Dimensions = dimensions;
    }

    public decimal GetProfitMargin() => SellingPrice.Subtract(CostPrice).Amount;

    public decimal GetProfitMarginPercentage() => CostPrice.Amount > 0 ? ((SellingPrice.Amount - CostPrice.Amount) / CostPrice.Amount) * 100 : 0;
}
