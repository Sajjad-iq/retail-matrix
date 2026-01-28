using Domains.Inventory.Enums;

namespace Application.Inventory.DTOs;

public record InventoryOperationListDto
{
    public Guid Id { get; init; }
    public InventoryOperationType OperationType { get; init; }
    public string OperationNumber { get; init; } = string.Empty;
    public DateTime OperationDate { get; init; }
    public Guid? SourceInventoryId { get; init; }
    public string? SourceInventoryName { get; init; }
    public Guid? DestinationInventoryId { get; init; }
    public string? DestinationInventoryName { get; init; }
    public Guid UserId { get; init; }
    public string UserName { get; init; } = string.Empty;
    public string? UserAvatar { get; init; }
    public Guid OrganizationId { get; init; }
    public InventoryOperationStatus Status { get; init; }
    public string? Notes { get; init; }
    public DateTime InsertDate { get; init; }
}
