namespace Application.Inventory.DTOs;

public record InventoryOperationItemDto
{
    public Guid Id { get; init; }
    public Guid ProductPackagingId { get; init; }
    public int Quantity { get; init; }
}
