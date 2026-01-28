using MediatR;

namespace Domains.Shared.Base;

/// <summary>
/// Base interface for all domain events
/// </summary>
public interface IDomainEvent : INotification
{
    DateTime OccurredOn { get; }
}
