using MediatR;

namespace Application.POS.Commands.CreateSale;

/// <summary>
/// Command to create a draft sale with items
/// </summary>
public record CreateSaleCommand : IRequest<Guid>
{
    public Guid InventoryId { get; init; }
    public List<SaleItemInput> Items { get; init; } = new();
    public string? Notes { get; init; }
}

/// <summary>
/// Input model for a sale item
/// </summary>
public record SaleItemInput
{
    public string? Barcode { get; init; }
    public Guid? ProductPackagingId { get; init; }
    public int Quantity { get; init; } = 1;
    public DiscountInput? Discount { get; init; }
}

/// <summary>
/// Input model for discount
/// </summary>
public record DiscountInput
{
    public decimal Amount { get; init; }
    public bool IsPercentage { get; init; }
}
