using MediatR;

namespace Application.Stocks.Commands.AdjustStockQuantity;

public record AdjustStockQuantityCommand : IRequest<bool>
{
    public Guid StockId { get; init; }
    public Guid BatchId { get; init; }
    public int QuantityChange { get; init; }
}
