using Domains.Common.Currency.Enums;

namespace Application.Currencies.DTOs;

public record CurrencyDto
{
    public Guid Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Symbol { get; init; } = string.Empty;
    public decimal ExchangeRate { get; init; }
    public bool IsBaseCurrency { get; init; }
    public CurrencyStatus Status { get; init; }
    public Guid OrganizationId { get; init; }
    public DateTime InsertDate { get; init; }
}
