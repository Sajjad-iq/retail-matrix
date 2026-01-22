using Application.POS.DTOs;
using MediatR;

namespace Application.POS.Commands.CompleteSale;

/// <summary>
/// Command to complete a sale with payment
/// </summary>
public record CompleteSaleCommand : IRequest<CompletedSaleDto>
{
    public Guid SaleId { get; init; }
    public Guid InventoryId { get; init; }
    public decimal AmountPaid { get; init; }
}
