namespace Domains.Payments.Enums;

/// <summary>
/// Represents the type of entity that a payment is associated with
/// </summary>
public enum PaymentEntityType
{
    Sale,
    Purchase,
    Invoice,
    Subscription,
    Refund,
    Other
}
