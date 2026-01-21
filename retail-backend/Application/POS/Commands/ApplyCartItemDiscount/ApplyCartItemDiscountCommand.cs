using MediatR;

namespace Application.POS.Commands.ApplyCartItemDiscount;

/// <summary>
/// Command to apply discount to a cart item
/// </summary>
public record ApplyCartItemDiscountCommand : IRequest<bool>
{
    public Guid SaleId { get; init; }
    public Guid ItemId { get; init; }
    public decimal DiscountValue { get; init; }
    public string DiscountType { get; init; } = "Percentage"; // Percentage or FixedAmount
}
