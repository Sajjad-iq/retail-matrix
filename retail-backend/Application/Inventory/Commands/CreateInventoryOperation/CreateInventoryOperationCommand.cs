using Domains.Inventory.Enums;
using MediatR;

namespace Application.Inventory.Commands.CreateInventoryOperation;

public record CreateInventoryOperationCommand : IRequest<Guid>
{
    public InventoryOperationType OperationType { get; init; }
    public string OperationNumber { get; init; } = string.Empty;
    public Guid? SourceInventoryId { get; init; }
    public Guid? DestinationInventoryId { get; init; }
    public string? Notes { get; init; }
}
