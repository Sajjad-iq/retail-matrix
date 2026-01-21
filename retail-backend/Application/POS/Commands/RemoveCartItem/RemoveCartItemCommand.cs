using MediatR;

namespace Application.POS.Commands.RemoveCartItem;

/// <summary>
/// Command to remove an item from the POS cart
/// </summary>
public record RemoveCartItemCommand : IRequest<bool>
{
    public Guid SaleId { get; init; }
    public Guid ItemId { get; init; }
}
