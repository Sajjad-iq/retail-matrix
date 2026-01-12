using Domains.Shared.ValueObjects;  // For Price
using Domains.Payments.Enums;
using Domains.Shared.Base;

namespace Domains.Payments.Entities;

/// <summary>
/// Payment entity - represents a payment transaction for any entity type
/// </summary>
public class Payment : BaseEntity
{
    // Parameterless constructor for EF Core
    private Payment()
    {
        Amount = Price.Create(0, "IQD");
    }

    // Private constructor to enforce factory methods
    private Payment(
        Guid entityId,
        PaymentEntityType entityType,
        PaymentMethod paymentMethod,
        Price amount,
        string? referenceNumber)
    {
        Id = Guid.NewGuid();
        EntityId = entityId;
        EntityType = entityType;
        PaymentMethod = paymentMethod;
        Amount = amount;
        PaymentDate = DateTime.UtcNow;
        ReferenceNumber = referenceNumber;
        Status = PaymentStatus.Completed;
        InsertDate = DateTime.UtcNow;
    }

    // Properties
    public Guid EntityId { get; private set; }
    public PaymentEntityType EntityType { get; private set; }
    public PaymentMethod PaymentMethod { get; private set; }
    public Price Amount { get; private set; }
    public DateTime PaymentDate { get; private set; }
    public string? ReferenceNumber { get; private set; }
    public PaymentStatus Status { get; private set; }

    // Factory method
    public static Payment Create(
        Guid entityId,
        PaymentEntityType entityType,
        PaymentMethod paymentMethod,
        Price amount,
        string? referenceNumber = null)
    {
        if (entityId == Guid.Empty)
            throw new ArgumentException("معرف الكيان مطلوب", nameof(entityId));

        if (amount.Amount <= 0)
            throw new ArgumentException("المبلغ يجب أن يكون أكبر من صفر", nameof(amount));

        return new Payment(
            entityId,
            entityType,
            paymentMethod,
            amount,
            referenceNumber
        );
    }

    // Domain methods
    public void MarkAsCompleted()
    {
        Status = PaymentStatus.Completed;
        UpdateDate = DateTime.UtcNow;
    }

    public void MarkAsFailed()
    {
        Status = PaymentStatus.Failed;
        UpdateDate = DateTime.UtcNow;
    }

    public void MarkAsRefunded()
    {
        if (Status != PaymentStatus.Completed)
            throw new InvalidOperationException("يمكن استرداد المدفوعات المكتملة فقط");

        Status = PaymentStatus.Refunded;
        UpdateDate = DateTime.UtcNow;
    }
}
