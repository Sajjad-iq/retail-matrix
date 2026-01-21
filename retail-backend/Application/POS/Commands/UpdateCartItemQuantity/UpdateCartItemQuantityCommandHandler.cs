using Application.Common.Exceptions;
using Domains.Common.Currency.Services;
using Domains.Sales.Repositories;
using Domains.Stocks.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.POS.Commands.UpdateCartItemQuantity;

public class UpdateCartItemQuantityCommandHandler : IRequestHandler<UpdateCartItemQuantityCommand, bool>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IStockRepository _stockRepository;
    private readonly ICurrencyConversionService _currencyService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UpdateCartItemQuantityCommandHandler(
        ISaleRepository saleRepository,
        IStockRepository stockRepository,
        ICurrencyConversionService currencyService,
        IHttpContextAccessor httpContextAccessor)
    {
        _saleRepository = saleRepository;
        _stockRepository = stockRepository;
        _currencyService = currencyService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<bool> Handle(UpdateCartItemQuantityCommand request, CancellationToken cancellationToken)
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

        // Check stock availability for the new quantity
        var stock = await _stockRepository.GetByPackagingAsync(
            item.ProductPackagingId,
            organizationId,
            request.InventoryId,
            cancellationToken);

        if (stock == null || stock.TotalAvailableQuantity < request.Quantity)
        {
            var available = stock?.TotalAvailableQuantity ?? 0;
            throw new ValidationException($"الكمية المتاحة في المخزون ({available}) أقل من المطلوبة ({request.Quantity})");
        }

        // Update the quantity using domain method
        await sale.UpdateItemQuantityAsync(
            request.ItemId,
            request.Quantity,
            _currencyService,
            cancellationToken
        );

        await _saleRepository.SaveChangesAsync(cancellationToken);
        return true;
    }
}
