using Domains.Inventory.Enums;

namespace Domains.Inventory.Models;

public record InventoryOperationFilter
{
    public string? SearchTerm { get; init; }
    public InventoryOperationStatus? Status { get; init; }
    public InventoryOperationType? Type { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public Guid? SourceInventoryId { get; init; }
    public Guid? DestinationInventoryId { get; init; }
}
