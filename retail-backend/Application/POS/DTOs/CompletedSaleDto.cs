using Domains.Shared.ValueObjects;

namespace Application.POS.DTOs;

/// <summary>
/// DTO for completed sale receipt data
/// </summary>
public record CompletedSaleDto
{
    public Guid SaleId { get; init; }
    public string SaleNumber { get; init; } = string.Empty;
    public DateTime SaleDate { get; init; }
    public DateTime CompletedAt { get; init; }
    public List<PosCartItemDto> Items { get; init; } = new();
    public Price TotalDiscount { get; init; } = null!;
    public Price GrandTotal { get; init; } = null!;
    public Price AmountPaid { get; init; } = null!;
    public int TotalItems => Items.Sum(i => i.Quantity);
}
