using MediatR;

namespace Application.Currencies.Commands.SetBaseCurrency;

public record SetBaseCurrencyCommand : IRequest<bool>
{
    public Guid CurrencyId { get; init; }
}
