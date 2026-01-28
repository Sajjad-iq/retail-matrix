using Domains.Stocks.Enums;

namespace Domains.Stocks.Models;

public record StockFilter
{
    public Guid? InventoryId { get; init; }
    public Guid? ProductId { get; init; }
    public Guid? ProductPackagingId { get; init; }
    public string? ProductName { get; init; }
    public StockStatus? Status { get; init; }
    public int? ReorderLevel { get; init; }
}
