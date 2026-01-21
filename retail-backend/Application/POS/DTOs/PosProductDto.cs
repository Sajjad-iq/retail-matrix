using Domains.Shared.ValueObjects;

namespace Application.POS.DTOs;

/// <summary>
/// DTO for product lookup result in POS (by barcode)
/// </summary>
public record PosProductDto
{
    public Guid ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public Guid PackagingId { get; init; }
    public string PackagingName { get; init; } = string.Empty;
    public string? Barcode { get; init; }
    public Price SellingPrice { get; init; } = null!;
    public Discount? Discount { get; init; }
    public Price DiscountedPrice { get; init; } = null!;
    public int AvailableStock { get; init; }
}
