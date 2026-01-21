using Application.Stocks.DTOs;
using MediatR;

namespace Application.Stocks.Queries.GetStockById;

public record GetStockByIdQuery : IRequest<StockDto>
{
    public Guid StockId { get; init; }
}
