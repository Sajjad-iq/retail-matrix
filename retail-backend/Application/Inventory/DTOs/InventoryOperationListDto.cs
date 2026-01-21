using Domains.Inventory.Enums;

namespace Application.Inventory.DTOs;

public record InventoryOperationListDto
{
    public Guid Id { get; init; }
    public InventoryOperationType OperationType { get; init; }
    public string OperationNumber { get; init; } = string.Empty;
    public DateTime OperationDate { get; init; }
    public InventoryOperationStatus Status { get; init; }
}
