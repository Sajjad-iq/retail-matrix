using Application.Common.Exceptions;
using Application.Common.Services;
using Domains.Common.Currency.Entities;
using Domains.Common.Currency.Repositories;
using MediatR;

namespace Application.Currencies.Commands.CreateCurrency;

public class CreateCurrencyCommandHandler : IRequestHandler<CreateCurrencyCommand, Guid>
{
    private readonly ICurrencyRepository _currencyRepository;
    private readonly IOrganizationContext _organizationContext;

    public CreateCurrencyCommandHandler(
        ICurrencyRepository currencyRepository,
        IOrganizationContext organizationContext)
    {
        _currencyRepository = currencyRepository;
        _organizationContext = organizationContext;
    }

    public async Task<Guid> Handle(CreateCurrencyCommand request, CancellationToken cancellationToken)
    {
        var organizationId = _organizationContext.OrganizationId;

        // Check if currency code already exists
        var exists = await _currencyRepository.ExistsByCodeAsync(request.Code, organizationId, cancellationToken);
        if (exists)
        {
            throw new ValidationException($"العملة بالرمز {request.Code} موجودة مسبقاً");
        }

        // If setting as base currency, ensure no other base currency exists
        if (request.IsBaseCurrency)
        {
            var existingBase = await _currencyRepository.GetBaseCurrencyAsync(organizationId, cancellationToken);
            if (existingBase != null)
            {
                throw new ValidationException("يوجد عملة أساسية مسبقاً. الرجاء إلغاء تفعيل العملة الأساسية الحالية أولاً");
            }
        }

        var currency = Currency.Create(
            request.Code,
            request.Name,
            request.Symbol,
            request.ExchangeRate,
            request.IsBaseCurrency,
            organizationId);

        // Update status if not active
        if (request.Status != Domains.Common.Currency.Enums.CurrencyStatus.Active)
        {
            currency.UpdateStatus(request.Status);
        }

        await _currencyRepository.AddAsync(currency, cancellationToken);
        await _currencyRepository.SaveChangesAsync(cancellationToken);

        return currency.Id;
    }
}
