using Domains.Sales.Enums;
using Domains.Shared.ValueObjects;

namespace Application.POS.DTOs;

/// <summary>
/// DTO for sales history list item
/// </summary>
public record PosSaleListDto
{
    public Guid SaleId { get; init; }
    public string SaleNumber { get; init; } = string.Empty;
    public DateTime SaleDate { get; init; }
    public SaleStatus Status { get; init; }
    public Price GrandTotal { get; init; } = null!;
    public int ItemCount { get; init; }
}
