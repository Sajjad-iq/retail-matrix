using Domains.Shared.ValueObjects;

namespace Application.POS.DTOs;

/// <summary>
/// DTO for a cart item in POS
/// </summary>
public record PosCartItemDto
{
    public Guid ItemId { get; init; }
    public Guid ProductPackagingId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public Price UnitPrice { get; init; } = null!;
    public Discount? Discount { get; init; }
    public Price LineTotal { get; init; } = null!;
}
