using Application.Common.Exceptions;
using Domains.Common.Currency.Services;
using Domains.Sales.Repositories;
using Domains.Shared.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.POS.Commands.ApplyCartItemDiscount;

public class ApplyCartItemDiscountCommandHandler : IRequestHandler<ApplyCartItemDiscountCommand, bool>
{
    private readonly ISaleRepository _saleRepository;
    private readonly ICurrencyConversionService _currencyService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ApplyCartItemDiscountCommandHandler(
        ISaleRepository saleRepository,
        ICurrencyConversionService currencyService,
        IHttpContextAccessor httpContextAccessor)
    {
        _saleRepository = saleRepository;
        _currencyService = currencyService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<bool> Handle(ApplyCartItemDiscountCommand request, CancellationToken cancellationToken)
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

        var item = sale.Items.FirstOrDefault(i => i.Id == request.ItemId);
        if (item == null)
        {
            throw new NotFoundException("عنصر البيع غير موجود");
        }

        // Create discount based on type
        Discount discount;
        if (request.DiscountType.Equals("Percentage", StringComparison.OrdinalIgnoreCase))
        {
            discount = Discount.Percentage(request.DiscountValue);
        }
        else if (request.DiscountType.Equals("FixedAmount", StringComparison.OrdinalIgnoreCase))
        {
            discount = Discount.FixedAmount(request.DiscountValue);
        }
        else
        {
            throw new ValidationException("نوع الخصم غير صالح. يجب أن يكون Percentage أو FixedAmount");
        }

        // Apply discount to item
        item.UpdateDiscount(discount);
        item.CalculateLineTotal();

        await _saleRepository.SaveChangesAsync(cancellationToken);
        return true;
    }
}
