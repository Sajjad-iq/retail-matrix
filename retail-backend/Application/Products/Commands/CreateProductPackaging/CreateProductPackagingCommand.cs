using Domains.Products.Enums;
using Domains.Shared.ValueObjects;
using MediatR;

namespace Application.Products.Commands.CreateProductPackaging;

public record CreateProductPackagingCommand : IRequest<Guid>
{
    public Guid? ProductId { get; init; }  // Optional - will auto-create if not provided
    public Guid? CategoryId { get; init; }  // For auto-created product

    // Product-level properties (for auto-created product)
    public string ProductName { get; init; } = string.Empty;
    public List<string>? ProductImageUrls { get; init; }

    // Packaging-level properties
    public string Name { get; init; } = string.Empty;
    public decimal SellingPriceAmount { get; init; }
    public string SellingPriceCurrency { get; init; } = "IQD";
    public UnitOfMeasure UnitOfMeasure { get; init; }
    public string? Barcode { get; init; }
    public string? Description { get; init; }
    public int UnitsPerPackage { get; init; } = 1;
    public bool IsDefault { get; init; }
    public List<string>? ImageUrls { get; init; }
    public string? Dimensions { get; init; }
    public Weight? Weight { get; init; }  // Using Weight value object
    public string? Color { get; init; }
}
