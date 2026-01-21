using Application.POS.DTOs;
using MediatR;

namespace Application.POS.Queries.GetPosCart;

/// <summary>
/// Query to get current POS cart state
/// </summary>
public record GetPosCartQuery : IRequest<PosCartDto>
{
    public Guid SaleId { get; init; }
}
