using Application.Stocks.DTOs;
using Domains.Shared.Base;
using MediatR;

namespace Application.Stocks.Queries.GetMyStocks;

public record GetMyStocksQuery : IRequest<PagedResult<StockListDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public Guid? InventoryId { get; init; }
    public StockStatus StockStatus { get; init; } = StockStatus.All;
    public int? ReorderLevel { get; init; }
}
