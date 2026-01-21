using Domains.Shared.ValueObjects;
using Domains.Stocks.Enums;
using MediatR;

namespace Application.Stocks.Commands.CreateStock;

public record CreateStockCommand : IRequest<Guid>
{
    public Guid ProductPackagingId { get; init; }
    public Guid InventoryId { get; init; }

    // Optional initial batch
    public string? InitialBatchNumber { get; init; }
    public int? InitialQuantity { get; init; }
    public DateTime? InitialExpiryDate { get; init; }
    public StockCondition? InitialCondition { get; init; }
    public Price? InitialCostPrice { get; init; }
}
