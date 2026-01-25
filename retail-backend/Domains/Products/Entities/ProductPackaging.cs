using Domains.Products.Enums;
using Domains.Shared.ValueObjects;
using Domains.Shared.Base;
using BarcodeVO = Domains.Shared.ValueObjects.Barcode;

namespace Domains.Products.Entities;

/// <summary>
/// Represents a packaging variant of a product with pricing
/// </summary>
public class ProductPackaging : BaseEntity
{
    // Parameterless constructor for EF Core
    private ProductPackaging()
    {
        Name = string.Empty;
        SellingPrice = null!;   // Will be set by EF Core
        Discount = null!;       // Will be set by EF Core
        ImageUrls = new List<string>();  // Initialize empty list
    }

    // Private constructor to enforce factory methods
    private ProductPackaging(
        Guid productId,
        string name,
        Price sellingPrice,
        UnitOfMeasure unitOfMeasure,
        BarcodeVO? barcode = null,
        string? description = null,
        int unitsPerPackage = 1,
        bool isDefault = false,
        List<string>? imageUrls = null,
        string? dimensions = null,
        Weight? weight = null,
        string? color = null,
        Discount? discount = null)
    {
        Id = Guid.NewGuid();
        ProductId = productId;
        Name = name;
        Barcode = barcode;
        Description = description;
        UnitsPerPackage = unitsPerPackage;
        UnitOfMeasure = unitOfMeasure;
        SellingPrice = sellingPrice;
        Discount = discount ?? Discount.None();
        IsDefault = isDefault;
        ImageUrls = imageUrls ?? new List<string>();
        Dimensions = dimensions;
        Weight = weight;
        Color = color;
        InsertDate = DateTime.UtcNow;
        Status = ProductStatus.Active;  // Default to Active
    }

    // Properties
    public Guid ProductId { get; private set; }
    public string Name { get; private set; }
    public BarcodeVO? Barcode { get; private set; }
    public string? Description { get; private set; }
    public int UnitsPerPackage { get; private set; }
    public UnitOfMeasure UnitOfMeasure { get; private set; }
    public Price SellingPrice { get; private set; }
    public Discount Discount { get; private set; }
    public bool IsDefault { get; private set; }
    public ProductStatus Status { get; private set; }
    public List<string> ImageUrls { get; private set; }
    public string? Dimensions { get; private set; } // Dimensions as string (e.g., "10x20x30 cm", "5x8x12 in")
    public Weight? Weight { get; private set; } // Weight of the package
    public string? Color { get; private set; } // Color of the package (e.g., "Red", "Blue", "#FF5733")

    // Navigation properties
    public Product? Product { get; private set; }

    /// <summary>
    /// Factory method to create a new product packaging
    /// </summary>
    public static ProductPackaging Create(
        Guid productId,
        string name,
        Price sellingPrice,
        UnitOfMeasure unitOfMeasure,
        string? barcode = null,
        string? description = null,
        int unitsPerPackage = 1,
        bool isDefault = false,
        List<string>? imageUrls = null,
        string? dimensions = null,
        Weight? weight = null,
        string? color = null,
        Discount? discount = null)
    {
        // Validate and create Barcode value object if provided
        BarcodeVO? barcodeVO = barcode != null ? BarcodeVO.Create(barcode) : null;

        if (productId == Guid.Empty)
            throw new ArgumentException("معرف المنتج مطلوب", nameof(productId));

        if (unitsPerPackage <= 0)
            throw new ArgumentException("عدد الوحدات في العبوة يجب أن يكون أكبر من صفر", nameof(unitsPerPackage));

        return new ProductPackaging(
            productId: productId,
            name: name,
            sellingPrice: sellingPrice,
            unitOfMeasure: unitOfMeasure,
            barcode: barcodeVO,
            description: description,
            unitsPerPackage: unitsPerPackage,
            isDefault: isDefault,
            imageUrls: imageUrls,
            dimensions: dimensions,
            weight: weight,
            color: color,
            discount: discount
        );
    }

    // Domain Methods
    public void UpdatePricing(Price sellingPrice)
    {
        SellingPrice = sellingPrice;
    }

    /// <summary>
    /// Updates the discount applied to this packaging
    /// </summary>
    public void UpdateDiscount(Discount discount)
    {
        Discount = discount ?? Discount.None();
    }

    /// <summary>
    /// Updates the discount with a percentage value
    /// </summary>
    public void SetPercentageDiscount(decimal percentage)
    {
        Discount = Discount.Percentage(percentage);
    }

    /// <summary>
    /// Updates the discount with a fixed amount value
    /// </summary>
    public void SetFixedAmountDiscount(decimal amount)
    {
        Discount = Discount.FixedAmount(amount);
    }

    /// <summary>
    /// Removes any discount applied to this packaging
    /// </summary>
    public void ClearDiscount()
    {
        Discount = Discount.None();
    }

    /// <summary>
    /// Gets the final price after applying discount
    /// </summary>
    public Price GetDiscountedPrice()
    {
        return Discount.ApplyTo(SellingPrice);
    }

    /// <summary>
    /// Gets the discount amount for the current selling price
    /// </summary>
    public decimal GetDiscountAmount()
    {
        return Discount.GetDiscountAmount(SellingPrice);
    }

    public void UpdateBarcode(string barcode)
    {
        Barcode = BarcodeVO.Create(barcode);
    }

    public void UpdatePackagingInfo(int unitsPerPackage, UnitOfMeasure unitOfMeasure)
    {
        if (unitsPerPackage <= 0)
            throw new ArgumentException("عدد الوحدات في العبوة يجب أن يكون أكبر من صفر", nameof(unitsPerPackage));

        UnitsPerPackage = unitsPerPackage;
        UnitOfMeasure = unitOfMeasure;
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
        Status = ProductStatus.Active;
    }

    public void Deactivate()
    {
        Status = ProductStatus.Inactive;
    }

    public void Discontinue()
    {
        Status = ProductStatus.Discontinued;
    }

    public void UpdateImages(List<string>? imageUrls)
    {
        ImageUrls = imageUrls ?? new List<string>();
    }

    public void UpdateDescription(string? description)
    {
        Description = description;
    }

    public void UpdateDimensions(string? dimensions)
    {
        Dimensions = dimensions;
    }

    public void UpdateWeight(Weight? weight)
    {
        Weight = weight;
    }

    public void UpdateColor(string? color)
    {
        Color = color;
    }

    public void UpdateInfo(
        string name,
        Price sellingPrice,
        UnitOfMeasure unitOfMeasure,
        string? barcode = null,
        string? description = null,
        int unitsPerPackage = 1,
        bool isDefault = false,
        List<string>? imageUrls = null,
        string? dimensions = null,
        Weight? weight = null,
        string? color = null)
    {
        Name = name;
        SellingPrice = sellingPrice;
        UnitOfMeasure = unitOfMeasure;
        Barcode = !string.IsNullOrWhiteSpace(barcode) ? BarcodeVO.Create(barcode) : null;
        Description = description;
        if (unitsPerPackage <= 0)
            throw new ArgumentException("عدد الوحدات في العبوة يجب أن يكون أكبر من صفر", nameof(unitsPerPackage));
        UnitsPerPackage = unitsPerPackage;
        IsDefault = isDefault;
        ImageUrls = imageUrls ?? new List<string>();
        Dimensions = dimensions;
        Weight = weight;
        Color = color;
    }
}
