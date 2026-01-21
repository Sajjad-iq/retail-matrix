using MediatR;

namespace Application.POS.Commands.UpdateCartItemQuantity;

/// <summary>
/// Command to update cart item quantity
/// </summary>
public record UpdateCartItemQuantityCommand : IRequest<bool>
{
    public Guid SaleId { get; init; }
    public Guid ItemId { get; init; }
    public int Quantity { get; init; }
    public Guid InventoryId { get; init; }
}
