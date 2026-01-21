using Domains.Inventory.Enums;

namespace Application.Inventory.DTOs;

public record InventoryDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Code { get; init; } = string.Empty;
    public InventoryType Type { get; init; }
    public Guid OrganizationId { get; init; }
    public Guid? ParentId { get; init; }
    public bool IsActive { get; init; }
    public DateTime InsertDate { get; init; }
}
