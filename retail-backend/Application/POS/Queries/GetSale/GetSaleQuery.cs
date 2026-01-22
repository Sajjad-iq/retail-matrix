using Application.POS.DTOs;
using MediatR;

namespace Application.POS.Queries.GetSale;

/// <summary>
/// Query to get sale details
/// </summary>
public record GetSaleQuery : IRequest<SaleDto>
{
    public Guid SaleId { get; init; }
}
