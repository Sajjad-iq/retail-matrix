using Domains.Shared.ValueObjects;
using Domains.Stocks.Enums;
using MediatR;

namespace Application.Stocks.Commands.AddStockBatch;

public record AddStockBatchCommand : IRequest<Guid>
{
    public Guid StockId { get; init; }
    public string BatchNumber { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public DateTime? ExpiryDate { get; init; }
    public StockCondition Condition { get; init; } = StockCondition.New;
    public Price? CostPrice { get; init; }
}
