using Domains.Products.Enums;
using Domains.Shared.ValueObjects;
using BarcodeVO = Domains.Shared.ValueObjects.Barcode;

namespace Application.Products.DTOs;

public record ProductPackagingListDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public BarcodeVO? Barcode { get; init; }
    public int UnitsPerPackage { get; init; }
    public UnitOfMeasure UnitOfMeasure { get; init; }
    public Price SellingPrice { get; init; } = null!;
    public Discount Discount { get; init; } = null!;
    public bool IsDefault { get; init; }
    public ProductStatus Status { get; init; }
    public List<string> ImageUrls { get; init; } = new();
    public string? Dimensions { get; init; }
    public Weight? Weight { get; init; }
    public string? Color { get; init; }
    public DateTime InsertDate { get; init; }
}
