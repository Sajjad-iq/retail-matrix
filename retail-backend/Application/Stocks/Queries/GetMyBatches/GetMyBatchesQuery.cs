using Application.Stocks.DTOs;
using Domains.Shared.Base;
using Domains.Stocks.Enums;
using MediatR;

namespace Application.Stocks.Queries.GetMyBatches;

public record GetMyBatchesQuery : IRequest<PagedResult<StockBatchDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public BatchStatus BatchStatus { get; init; } = BatchStatus.All;
    public int DaysThreshold { get; init; } = 30;
    public Guid? StockId { get; init; }
    public StockCondition? Condition { get; init; }
}
