using Domains.Common.Currency.Enums;
using MediatR;

namespace Application.Currencies.Commands.CreateCurrency;

public record CreateCurrencyCommand : IRequest<Guid>
{
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Symbol { get; init; } = string.Empty;
    public decimal ExchangeRate { get; init; }
    public bool IsBaseCurrency { get; init; }
    public CurrencyStatus Status { get; init; }
}
