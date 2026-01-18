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
    }

    private InstallmentPayment(
        Guid installmentPlanId,
        Price amount,
        string? reference,
        string? notes,
        Guid receivedByUserId)
    {
        Id = Guid.NewGuid();
        InstallmentPlanId = installmentPlanId;
        Amount = amount;
        PaymentDate = DateTime.UtcNow;
        Reference = reference;
        Notes = notes;
        ReceivedByUserId = receivedByUserId;
        InsertDate = DateTime.UtcNow;
    }

    // Properties
    public Guid InstallmentPlanId { get; private set; }
    public Price Amount { get; private set; }
    public DateTime PaymentDate { get; private set; }
    public string? Reference { get; private set; }
    public string? Notes { get; private set; }
    public Guid ReceivedByUserId { get; private set; }

    // Navigation
    public InstallmentPlan? InstallmentPlan { get; private set; }

    /// <summary>
    /// Factory method to create a new payment
    /// </summary>
    internal static InstallmentPayment Create(
        Guid installmentPlanId,
        Price amount,
        Guid receivedByUserId,
        string? reference = null,
        string? notes = null)
    {
        if (amount.Amount <= 0)
            throw new ArgumentException("مبلغ الدفعة يجب أن يكون أكبر من صفر", nameof(amount));

        return new InstallmentPayment(
            installmentPlanId,
            amount,
            reference,
            notes,
            receivedByUserId
        );
    }
}
