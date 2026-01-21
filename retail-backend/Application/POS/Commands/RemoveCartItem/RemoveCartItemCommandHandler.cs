using Application.Common.Exceptions;
using Domains.Common.Currency.Services;
using Domains.Sales.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.POS.Commands.RemoveCartItem;

public class RemoveCartItemCommandHandler : IRequestHandler<RemoveCartItemCommand, bool>
{
    private readonly ISaleRepository _saleRepository;
    private readonly ICurrencyConversionService _currencyService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RemoveCartItemCommandHandler(
        ISaleRepository saleRepository,
        ICurrencyConversionService currencyService,
        IHttpContextAccessor httpContextAccessor)
    {
        _saleRepository = saleRepository;
        _currencyService = currencyService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<bool> Handle(RemoveCartItemCommand request, CancellationToken cancellationToken)
    {
        var orgIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("OrganizationId")?.Value;
        if (string.IsNullOrEmpty(orgIdClaim) || !Guid.TryParse(orgIdClaim, out var organizationId))
        {
            throw new UnauthorizedException("معرف المؤسسة مطلوب");
        }

        var sale = await _saleRepository.GetByIdWithTrackingAsync(request.SaleId, cancellationToken);
        if (sale == null)
        {
            throw new NotFoundException("جلسة البيع غير موجودة");
        }

        if (sale.OrganizationId != organizationId)
        {
            throw new UnauthorizedException("غير مصرح بالوصول إلى هذه الجلسة");
        }

        // Remove item using domain method
        await sale.RemoveItemAsync(
            request.ItemId,
            _currencyService,
            cancellationToken
        );

        await _saleRepository.SaveChangesAsync(cancellationToken);
        return true;
    }
}
