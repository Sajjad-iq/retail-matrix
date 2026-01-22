using MediatR;

namespace Application.POS.Commands.CancelSale;

/// <summary>
/// Command to cancel a POS session
/// </summary>
public record CancelSaleCommand : IRequest<bool>
{
    public Guid SaleId { get; init; }
}
