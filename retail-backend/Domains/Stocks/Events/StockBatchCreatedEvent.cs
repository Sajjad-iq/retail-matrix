using Domains.Shared.Base;
using Domains.Stocks.Enums;
using Domains.Shared.ValueObjects;

namespace Domains.Stocks.Events;

/// <summary>
/// Domain event raised when a new stock batch is created
/// </summary>
public record StockBatchCreatedEvent(
    Guid StockId,
    Guid BatchId,
    Guid InventoryId,
    Guid ProductPackagingId,
    Guid OrganizationId,
    string BatchNumber,
    int Quantity,
    DateTime? ExpiryDate,
    StockCondition Condition,
    Price? CostPrice
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
