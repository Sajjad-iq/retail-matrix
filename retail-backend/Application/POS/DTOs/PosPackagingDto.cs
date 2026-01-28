using Domains.Products.Enums;
using Domains.Shared.ValueObjects;

namespace Application.POS.DTOs;

/// <summary>
/// DTO for packaging information in POS with full domain details and stock availability
/// </summary>
public record PosPackagingDto
{
    // Packaging Information
    public Guid PackagingId { get; init; }
    public string PackagingName { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? Barcode { get; init; }
    public int UnitsPerPackage { get; init; }
    public UnitOfMeasure UnitOfMeasure { get; init; }
    public bool IsDefault { get; init; }
    public ProductStatus Status { get; init; }
    public List<string> ImageUrls { get; init; } = new();
    public string? Dimensions { get; init; }
    public Weight? Weight { get; init; }
    public string? Color { get; init; }
    
    // Pricing Information
    public Price SellingPrice { get; init; } = null!;
    public Discount? Discount { get; init; }
    public Price DiscountedPrice { get; init; } = null!;
    
    // Stock Information (for specific inventory)
    public int AvailableStock { get; init; }
    
    // Computed Properties
    public bool InStock => AvailableStock > 0;
    public bool HasDiscount => Discount != null && Discount.Value > 0;
    public decimal DiscountPercentage => HasDiscount && Discount!.Type == Domains.Products.Enums.DiscountType.Percentage 
        ? Discount.Value 
        : 0;
}
