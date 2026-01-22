using Domains.Shared.ValueObjects;

namespace Application.POS.DTOs;

/// <summary>
/// DTO for sale details
/// </summary>
public record SaleDto
{
    public Guid SaleId { get; init; }
    public string SaleNumber { get; init; } = string.Empty;
    public DateTime SaleDate { get; init; }
    public string Status { get; init; } = string.Empty;
    public List<PosCartItemDto> Items { get; init; } = new();
    public Price TotalDiscount { get; init; } = null!;
    public Price GrandTotal { get; init; } = null!;
    public Price AmountPaid { get; init; } = null!;
    public string? Notes { get; init; }
    public int TotalItems => Items.Sum(i => i.Quantity);
}
