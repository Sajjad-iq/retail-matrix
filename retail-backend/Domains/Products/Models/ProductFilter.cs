using Domains.Products.Enums;

namespace Domains.Products.Models;

public record ProductFilter
{
    public List<Guid>? Ids { get; init; }
    public Guid? CategoryId { get; init; }
    public List<Guid>? CategoryIds { get; init; }
    public string? Name { get; init; }
    public string? SearchTerm { get; init; }
    public ProductStatus? Status { get; init; }
    public string? Barcode { get; init; }
    public bool? IsSelling { get; init; }
    public bool? IsRawMaterial { get; init; }
}
