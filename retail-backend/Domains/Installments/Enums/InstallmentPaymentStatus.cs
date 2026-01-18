namespace Domains.Installments.Enums;

/// <summary>
/// Status of an installment payment
/// </summary>
public enum InstallmentPaymentStatus
{
    /// <summary>
    /// Payment is scheduled but not yet paid
    /// </summary>
    Pending,

    /// <summary>
    /// Payment has been received
    /// </summary>
    Paid,

    /// <summary>
    /// Payment is overdue
    /// </summary>
    Overdue
}
