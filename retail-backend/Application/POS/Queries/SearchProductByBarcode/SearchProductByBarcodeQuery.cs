using Application.POS.DTOs;
using MediatR;

namespace Application.POS.Queries.SearchProductByBarcode;

/// <summary>
/// Query to search product by barcode for POS
/// </summary>
public record SearchProductByBarcodeQuery : IRequest<PosProductDto?>
{
    public string Barcode { get; init; } = string.Empty;
    public Guid InventoryId { get; init; }
}
