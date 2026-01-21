namespace Application.Stocks.DTOs;

public record StockDto
{
    public Guid Id { get; init; }
    public Guid ProductPackagingId { get; init; }
    public Guid InventoryId { get; init; }
    public Guid OrganizationId { get; init; }
    public int TotalQuantity { get; init; }
    public int TotalReservedQuantity { get; init; }
    public int TotalAvailableQuantity { get; init; }
    public DateTime? LastStocktakeDate { get; init; }
    public List<StockBatchDto> Batches { get; init; } = new();
    public DateTime InsertDate { get; init; }
}
