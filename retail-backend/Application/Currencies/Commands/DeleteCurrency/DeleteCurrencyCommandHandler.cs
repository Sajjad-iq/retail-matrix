using Application.Common.Exceptions;
using Application.Common.Services;
using Domains.Common.Currency.Repositories;
using MediatR;

namespace Application.Currencies.Commands.DeleteCurrency;

public class DeleteCurrencyCommandHandler : IRequestHandler<DeleteCurrencyCommand, bool>
{
    private readonly ICurrencyRepository _currencyRepository;
    private readonly IOrganizationContext _organizationContext;

    public DeleteCurrencyCommandHandler(
        ICurrencyRepository currencyRepository,
        IOrganizationContext organizationContext)
    {
        _currencyRepository = currencyRepository;
        _organizationContext = organizationContext;
    }

    public async Task<bool> Handle(DeleteCurrencyCommand request, CancellationToken cancellationToken)
    {
        var organizationId = _organizationContext.OrganizationId;

        var currency = await _currencyRepository.GetByIdAsync(request.Id, cancellationToken);
        if (currency == null)
        {
            throw new NotFoundException("العملة غير موجودة");
        }

        if (currency.OrganizationId != organizationId)
        {
            throw new UnauthorizedException("غير مصرح لك بحذف هذه العملة");
        }

        if (currency.IsBaseCurrency)
        {
            throw new ValidationException("لا يمكن حذف العملة الأساسية");
        }

        await _currencyRepository.DeleteAsync(currency.Id, cancellationToken);
        await _currencyRepository.SaveChangesAsync(cancellationToken);

        return true;
    }
}
