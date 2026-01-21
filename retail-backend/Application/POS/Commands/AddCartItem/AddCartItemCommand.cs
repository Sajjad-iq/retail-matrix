using MediatR;

namespace Application.POS.Commands.AddCartItem;

/// <summary>
/// Command to add an item to the POS cart
/// </summary>
public record AddCartItemCommand : IRequest<Guid>
{
    public Guid SaleId { get; init; }
    public string? Barcode { get; init; }
    public Guid? ProductPackagingId { get; init; }
    public int Quantity { get; init; } = 1;
    public Guid InventoryId { get; init; }
}
