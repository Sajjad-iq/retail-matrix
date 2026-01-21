using Application.Inventory.DTOs;
using MediatR;

namespace Application.Inventory.Queries.GetInventoryOperationById;

public record GetInventoryOperationByIdQuery : IRequest<InventoryOperationDto>
{
    public Guid OperationId { get; init; }
}
