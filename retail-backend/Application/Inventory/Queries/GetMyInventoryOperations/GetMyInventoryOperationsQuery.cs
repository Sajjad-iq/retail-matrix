using Application.Inventory.DTOs;
using Domains.Shared.Base;
using MediatR;

namespace Application.Inventory.Queries.GetMyInventoryOperations;

public record GetMyInventoryOperationsQuery : IRequest<PagedResult<InventoryOperationListDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
