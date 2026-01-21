using Domains.Sales.Enums;
using Domains.Shared.ValueObjects;

namespace Application.POS.DTOs;

/// <summary>
/// DTO for POS cart/session state
/// </summary>
public record PosCartDto
{
    public Guid SaleId { get; init; }
    public string SaleNumber { get; init; } = string.Empty;
    public DateTime SaleDate { get; init; }
    public SaleStatus Status { get; init; }
    public List<PosCartItemDto> Items { get; init; } = new();
    public Price TotalDiscount { get; init; } = null!;
    public Price GrandTotal { get; init; } = null!;
    public Price AmountPaid { get; init; } = null!;
    public decimal AmountDue => GrandTotal.Amount - AmountPaid.Amount;
    public int TotalItems => Items.Sum(i => i.Quantity);
    public string? Notes { get; init; }
}
