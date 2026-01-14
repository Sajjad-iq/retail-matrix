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
        SellingPrice = null!;   // Will be set by EF Core
        ImageUrls = new List<string>();  // Initialize empty list
    }

    // Private constructor to enforce factory methods
    private ProductPackaging(
        Guid productId,
        Price sellingPrice,
        UnitOfMeasure unitOfMeasure,
        Guid organizationId,
        string? barcode = null,
        string? description = null,
        int unitsPerPackage = 1,
        int reorderLevel = 10,
        bool isDefault = false,
        List<string>? imageUrls = null,
        string? dimensions = null,
        Weight? weight = null,
        string? color = null)
    {
        Id = Guid.NewGuid();
        ProductId = productId;
        Barcode = barcode;
        Description = description;
        UnitsPerPackage = unitsPerPackage;
        UnitOfMeasure = unitOfMeasure;
        SellingPrice = sellingPrice;
        ReorderLevel = reorderLevel;
        IsDefault = isDefault;
        ImageUrls = imageUrls ?? new List<string>();
        Dimensions = dimensions;
        Weight = weight;
        Color = color;
        OrganizationId = organizationId;
        InsertDate = DateTime.UtcNow;
        Status = ProductStatus.Active;  // Default to Active
    }

    // Properties
    public Guid ProductId { get; private set; }
    public string? Barcode { get; private set; }
    public string? Description { get; private set; }
    public int UnitsPerPackage { get; private set; }
    public UnitOfMeasure UnitOfMeasure { get; private set; }
    public Price SellingPrice { get; private set; }
    public int ReorderLevel { get; private set; }
    public bool IsDefault { get; private set; }
    public ProductStatus Status { get; private set; }
    public List<string> ImageUrls { get; private set; }
    public string? Dimensions { get; private set; } // Dimensions as string (e.g., "10x20x30 cm", "5x8x12 in")
    public Weight? Weight { get; private set; } // Weight of the package
    public string? Color { get; private set; } // Color of the package (e.g., "Red", "Blue", "#FF5733")
    public Guid OrganizationId { get; private set; }

    // Navigation properties
    public Product? Product { get; private set; }

    /// <summary>
    /// Factory method to create a new product packaging
    /// </summary>
    public static ProductPackaging Create(
        Guid productId,
        Price sellingPrice,
        UnitOfMeasure unitOfMeasure,
        Guid organizationId,
        string? barcode = null,
        string? description = null,
        int unitsPerPackage = 1,
        int reorderLevel = 10,
        bool isDefault = false,
        List<string>? imageUrls = null,
        string? dimensions = null,
        Weight? weight = null,
        string? color = null)
    {
        // Validate Barcode if provided
        if (barcode != null)
        {
            _ = BarcodeVO.Create(barcode); // Validate only
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
            sellingPrice: sellingPrice,
            unitOfMeasure: unitOfMeasure,
            organizationId: organizationId,
            barcode: barcode,
            description: description,
            unitsPerPackage: unitsPerPackage,
            reorderLevel: reorderLevel,
            isDefault: isDefault,
            imageUrls: imageUrls,
            dimensions: dimensions,
            weight: weight,
            color: color
        );
    }

    // Domain Methods
    public void UpdatePricing(Price sellingPrice)
    {
        SellingPrice = sellingPrice;
    }

    public void UpdateBarcode(string barcode)
    {
        var productBarcode = BarcodeVO.Create(barcode);
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

}
