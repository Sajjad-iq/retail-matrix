using Domains.Shared.ValueObjects;
using MediatR;

namespace Application.POS.Commands.RecordPosPayment;

/// <summary>
/// Command to record a payment for a POS session
/// </summary>
public record RecordPosPaymentCommand : IRequest<bool>
{
    public Guid SaleId { get; init; }
    public Price Amount { get; init; } = null!;
    public string PaymentMethod { get; init; } = "Cash"; // Cash, Card, BankTransfer, MobilePayment, Other
}
