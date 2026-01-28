using Domains.Stocks.Enums;

namespace Domains.Stocks.Models;

public record StockBatchFilter
{
    public StockCondition? Condition { get; init; }
    public int? DaysToExpiry { get; init; } // For "Near Expiry"
    public bool? IsExpired { get; init; }   // For "Expired"
}
