using Application.Inventory.DTOs;
using MediatR;

namespace Application.Inventory.Queries.GetInventoryOperationItems;

public record GetInventoryOperationItemsQuery : IRequest<List<InventoryOperationItemDto>>
{
    public Guid OperationId { get; init; }
}
