using Domains.Common.Currency.Enums;
using MediatR;

namespace Application.Currencies.Commands.UpdateCurrency;

public record UpdateCurrencyCommand : IRequest<bool>
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Symbol { get; init; } = string.Empty;
    public decimal ExchangeRate { get; init; }
    public CurrencyStatus Status { get; init; }
}
