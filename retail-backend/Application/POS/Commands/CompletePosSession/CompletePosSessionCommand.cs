using Application.POS.DTOs;
using MediatR;

namespace Application.POS.Commands.CompletePosSession;

/// <summary>
/// Command to complete a POS session (finalize sale and deduct stock)
/// </summary>
public record CompletePosSessionCommand : IRequest<CompletedSaleDto>
{
    public Guid SaleId { get; init; }
    public Guid InventoryId { get; init; }
}
