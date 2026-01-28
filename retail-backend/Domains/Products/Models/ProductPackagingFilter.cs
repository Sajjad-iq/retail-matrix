using Domains.Products.Enums;

namespace Domains.Products.Models;

public record ProductPackagingFilter
{
    public Guid? ProductId { get; init; }
    public ProductStatus? Status { get; init; }
    public string? SearchTerm { get; init; }
    public string? Barcode { get; init; }
    public bool? IsDefault { get; init; }
}
