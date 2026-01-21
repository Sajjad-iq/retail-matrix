using Domains.Shared.ValueObjects;
using Domains.Stocks.Enums;

namespace Application.Stocks.DTOs;

public record StockBatchDto
{
    public Guid Id { get; init; }
    public Guid StockId { get; init; }
    public string BatchNumber { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public int ReservedQuantity { get; init; }
    public int AvailableQuantity { get; init; }
    public DateTime? ExpiryDate { get; init; }
    public StockCondition Condition { get; init; }
    public Price? CostPrice { get; init; }
    public bool IsExpired { get; init; }
    public int? DaysUntilExpiry { get; init; }
    public DateTime InsertDate { get; init; }
}
