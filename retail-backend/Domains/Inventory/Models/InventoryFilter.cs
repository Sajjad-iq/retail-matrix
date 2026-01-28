using Domains.Inventory.Enums;

namespace Domains.Inventory.Models;

public record InventoryFilter
{
    public string? SearchTerm { get; init; }
    public InventoryType? Type { get; init; }
    public Guid? ParentId { get; init; }
    public bool? IsActive { get; init; }
}
