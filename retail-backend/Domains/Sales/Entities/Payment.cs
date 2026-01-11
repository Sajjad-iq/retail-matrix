using Domains.Shared.ValueObjects;  // For Price
using Domains.Sales.Enums;
using Domains.Shared.Base;

namespace Domains.Sales.Entities;

/// <summary>
/// Payment entity - represents a payment transaction for a sale
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
        Guid saleId,
        PaymentMethod paymentMethod,
        Price amount,
        string? referenceNumber)
    {
        Id = Guid.NewGuid();
        SaleId = saleId;
        PaymentMethod = paymentMethod;
        Amount = amount;
        PaymentDate = DateTime.UtcNow;
        ReferenceNumber = referenceNumber;
        Status = PaymentStatus.Completed;
        InsertDate = DateTime.UtcNow;
    }

    // Properties
    public Guid SaleId { get; private set; }
    public PaymentMethod PaymentMethod { get; private set; }
    public Price Amount { get; private set; }
    public DateTime PaymentDate { get; private set; }
    public string? ReferenceNumber { get; private set; }
    public PaymentStatus Status { get; private set; }

    // Factory method
    public static Payment Create(
        Guid saleId,
        PaymentMethod paymentMethod,
        Price amount,
        string? referenceNumber = null)
    {
        if (amount.Amount <= 0)
            throw new ArgumentException("المبلغ يجب أن يكون أكبر من صفر", nameof(amount));

        return new Payment(
            saleId,
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
