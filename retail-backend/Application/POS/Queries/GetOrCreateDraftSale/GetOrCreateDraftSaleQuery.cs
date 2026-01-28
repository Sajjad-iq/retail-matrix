using Application.POS.DTOs;
using MediatR;

namespace Application.POS.Queries.GetOrCreateDraftSale;

/// <summary>
/// Query to get existing draft sale or create a new one for the inventory
/// </summary>
public record GetOrCreateDraftSaleQuery : IRequest<SaleDto>
{
    public Guid InventoryId { get; init; }
}
