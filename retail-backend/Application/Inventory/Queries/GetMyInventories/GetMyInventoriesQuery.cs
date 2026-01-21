using Application.Inventory.DTOs;
using Domains.Shared.Base;
using MediatR;

namespace Application.Inventory.Queries.GetMyInventories;

public record GetMyInventoriesQuery : IRequest<PagedResult<InventoryDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
