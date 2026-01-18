using Domains.Installments.Enums;
using Domains.Shared.ValueObjects;
using Domains.Shared.Base;

namespace Domains.Installments.Entities;

/// <summary>
/// Represents a payment made towards an installment plan
/// </summary>
public class InstallmentPayment : BaseEntity
{
    // Parameterless constructor for EF Core
    private InstallmentPayment()
    {
        Amount = null!;
        PaidAmount = null!;
    }

    private InstallmentPayment(
        Guid installmentPlanId,
        Price amount,
        DateTime dueDate,
        int installmentNumber,
        InstallmentPaymentStatus status = InstallmentPaymentStatus.Pending,
        DateTime? paymentDate = null,
        string? reference = null,
        string? notes = null,
        Guid? receivedByUserId = null)
    {
        Id = Guid.NewGuid();
        InstallmentPlanId = installmentPlanId;
        Amount = amount;
        PaidAmount = Price.Create(0, amount.Currency);
        DueDate = dueDate;
        InstallmentNumber = installmentNumber;
        Status = status;
        PaymentDate = paymentDate;
        Reference = reference;
        Notes = notes;
        ReceivedByUserId = receivedByUserId;
        InsertDate = DateTime.UtcNow;
    }

    // Properties
    public Guid InstallmentPlanId { get; private set; }
    public Price Amount { get; private set; }
    public Price PaidAmount { get; private set; }
    public DateTime DueDate { get; private set; }
    public int InstallmentNumber { get; private set; }
    public InstallmentPaymentStatus Status { get; private set; }
    public DateTime? PaymentDate { get; private set; }
    public string? Reference { get; private set; }
    public string? Notes { get; private set; }
    public Guid? ReceivedByUserId { get; private set; }

    // Navigation
    public InstallmentPlan? InstallmentPlan { get; private set; }

    /// <summary>
    /// Creates a scheduled payment (not yet paid)
    /// </summary>
    internal static InstallmentPayment CreateScheduled(
        Guid installmentPlanId,
        Price amount,
        DateTime dueDate,
        int installmentNumber)
    {
        if (amount.Amount <= 0)
            throw new ArgumentException("مبلغ الدفعة يجب أن يكون أكبر من صفر", nameof(amount));

        return new InstallmentPayment(
            installmentPlanId,
            amount,
            dueDate,
            installmentNumber,
            InstallmentPaymentStatus.Pending
        )
        {
            PaidAmount = Price.Create(0, amount.Currency)
        };
    }

    /// <summary>
    /// Records a partial payment towards this installment
    /// </summary>
    public void RecordPartialPayment(Price amount, Guid receivedByUserId, string? reference = null, string? notes = null)
    {
        if (Status == InstallmentPaymentStatus.Paid)
            throw new InvalidOperationException("الدفعة مدفوعة بالفعل");

        PaidAmount = PaidAmount.Add(amount);

        if (PaidAmount.Amount >= Amount.Amount)
        {
            Status = InstallmentPaymentStatus.Paid;
            PaymentDate = DateTime.UtcNow;
        }

        ReceivedByUserId = receivedByUserId;
        Reference = reference;
        Notes = notes;
    }

    /// <summary>
    /// Gets the remaining amount to be paid for this installment
    /// </summary>
    public Price GetRemainingAmount()
    {
        return Amount.Subtract(PaidAmount);
    }

    /// <summary>
    /// Marks this payment as paid
    /// </summary>
    public void MarkAsPaid(Guid receivedByUserId, string? reference = null, string? notes = null)
    {
        if (Status == InstallmentPaymentStatus.Paid)
            throw new InvalidOperationException("الدفعة مدفوعة بالفعل");

        Status = InstallmentPaymentStatus.Paid;
        PaymentDate = DateTime.UtcNow;
        ReceivedByUserId = receivedByUserId;
        Reference = reference;
        Notes = notes;
    }

    /// <summary>
    /// Checks if payment is overdue
    /// </summary>
    public bool IsOverdue()
    {
        return Status == InstallmentPaymentStatus.Pending && DueDate < DateTime.UtcNow;
    }
}
