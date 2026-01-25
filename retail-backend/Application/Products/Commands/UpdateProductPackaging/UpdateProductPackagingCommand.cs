using Domains.Products.Enums;
using Domains.Shared.ValueObjects;
using MediatR;

namespace Application.Products.Commands.UpdateProductPackaging;

public record UpdateProductPackagingCommand : IRequest<Unit>
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public Price SellingPrice { get; init; } = null!;
    public UnitOfMeasure UnitOfMeasure { get; init; }
    public string? Barcode { get; init; }
    public string? Description { get; init; }
    public int UnitsPerPackage { get; init; }
    public bool IsDefault { get; init; }
    public List<string>? ImageUrls { get; init; }
    public string? Dimensions { get; init; }
    public Weight? Weight { get; init; }
    public string? Color { get; init; }
}
