using Domains.Shared.Base;

namespace Domains.Stocks.Events;

/// <summary>
/// Domain event raised when a stock batch quantity is adjusted
/// </summary>
public record StockBatchQuantityAdjustedEvent(
    Guid StockId,
    Guid BatchId,
    Guid InventoryId,
    Guid ProductPackagingId,
    Guid OrganizationId,
    string BatchNumber,
    int OldQuantity,
    int NewQuantity,
    int Adjustment,
    string? Reason
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
