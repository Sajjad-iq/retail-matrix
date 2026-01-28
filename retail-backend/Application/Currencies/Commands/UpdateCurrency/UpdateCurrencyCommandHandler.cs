using Application.Common.Exceptions;
using Application.Common.Services;
using Domains.Common.Currency.Repositories;
using MediatR;

namespace Application.Currencies.Commands.UpdateCurrency;

public class UpdateCurrencyCommandHandler : IRequestHandler<UpdateCurrencyCommand, bool>
{
    private readonly ICurrencyRepository _currencyRepository;
    private readonly IOrganizationContext _organizationContext;

    public UpdateCurrencyCommandHandler(
        ICurrencyRepository currencyRepository,
        IOrganizationContext organizationContext)
    {
        _currencyRepository = currencyRepository;
        _organizationContext = organizationContext;
    }

    public async Task<bool> Handle(UpdateCurrencyCommand request, CancellationToken cancellationToken)
    {
        var organizationId = _organizationContext.OrganizationId;

        var currency = await _currencyRepository.GetByIdAsync(request.Id, cancellationToken);
        if (currency == null)
        {
            throw new NotFoundException("العملة غير موجودة");
        }

        if (currency.OrganizationId != organizationId)
        {
            throw new UnauthorizedException("غير مصرح لك بتعديل هذه العملة");
        }

        // Update basic info
        currency.UpdateInfo(request.Name, request.Symbol);

        // Update exchange rate if changed
        if (currency.ExchangeRate != request.ExchangeRate)
        {
            currency.UpdateExchangeRate(request.ExchangeRate);
        }

        // Update status if changed
        if (currency.Status != request.Status)
        {
            currency.UpdateStatus(request.Status);
        }

        await _currencyRepository.UpdateAsync(currency, cancellationToken);
        await _currencyRepository.SaveChangesAsync(cancellationToken);

        return true;
    }
}
