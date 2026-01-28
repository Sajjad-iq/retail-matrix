using Application.Common.Exceptions;
using Application.Common.Services;
using Domains.Common.Currency.Repositories;
using MediatR;

namespace Application.Currencies.Commands.SetBaseCurrency;

public class SetBaseCurrencyCommandHandler : IRequestHandler<SetBaseCurrencyCommand, bool>
{
    private readonly ICurrencyRepository _currencyRepository;
    private readonly IOrganizationContext _organizationContext;

    public SetBaseCurrencyCommandHandler(
        ICurrencyRepository currencyRepository,
        IOrganizationContext organizationContext)
    {
        _currencyRepository = currencyRepository;
        _organizationContext = organizationContext;
    }

    public async Task<bool> Handle(SetBaseCurrencyCommand request, CancellationToken cancellationToken)
    {
        var organizationId = _organizationContext.OrganizationId;

        var currency = await _currencyRepository.GetByIdAsync(request.CurrencyId, cancellationToken);
        if (currency == null)
        {
            throw new NotFoundException("العملة غير موجودة");
        }

        if (currency.OrganizationId != organizationId)
        {
            throw new UnauthorizedException("غير مصرح لك بتعديل هذه العملة");
        }

        // Unset any existing base currency
        var existingBase = await _currencyRepository.GetBaseCurrencyAsync(organizationId, cancellationToken);
        if (existingBase != null && existingBase.Id != currency.Id)
        {
            existingBase.SetBaseCurrency(false);
            await _currencyRepository.UpdateAsync(existingBase, cancellationToken);
        }

        // Set new base currency
        currency.SetBaseCurrency(true);
        await _currencyRepository.UpdateAsync(currency, cancellationToken);
        await _currencyRepository.SaveChangesAsync(cancellationToken);

        return true;
    }
}
