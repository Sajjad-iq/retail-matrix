using Domains.Shared.ValueObjects;

namespace Application.Inventory.DTOs;

public record InventoryOperationItemDto
{
    public Guid Id { get; init; }
    public Guid ProductPackagingId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public string? Barcode { get; init; }
    public int Quantity { get; init; }
    public Price UnitPrice { get; init; } = null!;
    public string? Notes { get; init; }
}
