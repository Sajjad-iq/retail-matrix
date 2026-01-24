using Application.Common.Exceptions;
using Application.Common.Services;
using Domains.Sales.Repositories;
using MediatR;

namespace Application.POS.Commands.CancelSale;

public class CancelSaleCommandHandler : IRequestHandler<CancelSaleCommand, bool>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IOrganizationContext _organizationContext;

    public CancelSaleCommandHandler(
        ISaleRepository saleRepository,
        IOrganizationContext organizationContext)
    {
        _saleRepository = saleRepository;
        _organizationContext = organizationContext;
    }

    public async Task<bool> Handle(CancelSaleCommand request, CancellationToken cancellationToken)
    {
        var organizationId = _organizationContext.OrganizationId;

        var sale = await _saleRepository.GetByIdWithTrackingAsync(request.SaleId, cancellationToken);
        if (sale == null)
        {
            throw new NotFoundException("جلسة البيع غير موجودة");
        }

        if (sale.OrganizationId != organizationId)
        {
            throw new UnauthorizedException("غير مصرح بالوصول إلى هذه الجلسة");
        }

        // Cancel the sale using domain method
        sale.CancelSale();

        await _saleRepository.SaveChangesAsync(cancellationToken);
        return true;
    }
}
