using Application.Stocks.DTOs;
using Domains.Shared.Base;
using MediatR;

namespace Application.Stocks.Queries.GetMyStocks;

public record GetMyStocksQuery : IRequest<PagedResult<StockListDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public Guid? InventoryId { get; init; }
    public Guid? ProductId { get; init; }
    public Guid? ProductPackagingId { get; init; }
    public string? ProductName { get; init; }
    public Domains.Stocks.Enums.StockStatus StockStatus { get; init; } = Domains.Stocks.Enums.StockStatus.All;
    public int? ReorderLevel { get; init; }
}
