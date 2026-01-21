namespace Application.Stocks.DTOs;

public record StockListDto
{
    public Guid Id { get; init; }
    public Guid ProductPackagingId { get; init; }
    public Guid InventoryId { get; init; }
    public int TotalQuantity { get; init; }
    public int TotalAvailableQuantity { get; init; }
    public DateTime InsertDate { get; init; }
}
