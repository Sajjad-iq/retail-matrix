using MediatR;

namespace Application.Currencies.Commands.DeleteCurrency;

public record DeleteCurrencyCommand : IRequest<bool>
{
    public Guid Id { get; init; }
}
