using Application.Currencies.DTOs;
using Domains.Common.Currency.Enums;
using Domains.Shared.Base;
using MediatR;

namespace Application.Currencies.Queries.GetMyCurrencies;

public record GetMyCurrenciesQuery : IRequest<PagedResult<CurrencyDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public CurrencyStatus? Status { get; init; }
    public string? SearchTerm { get; init; }
}
