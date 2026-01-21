using Application.Common.Exceptions;
using Domains.Common.Currency.Services;
using Domains.Products.Repositories;
using Domains.Sales.Repositories;
using Domains.Stocks.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.POS.Commands.AddCartItem;

public class AddCartItemCommandHandler : IRequestHandler<AddCartItemCommand, Guid>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IProductPackagingRepository _productPackagingRepository;
    private readonly IStockRepository _stockRepository;
    private readonly ICurrencyConversionService _currencyService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AddCartItemCommandHandler(
        ISaleRepository saleRepository,
        IProductPackagingRepository productPackagingRepository,
        IStockRepository stockRepository,
        ICurrencyConversionService currencyService,
        IHttpContextAccessor httpContextAccessor)
    {
        _saleRepository = saleRepository;
        _productPackagingRepository = productPackagingRepository;
        _stockRepository = stockRepository;
        _currencyService = currencyService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Guid> Handle(AddCartItemCommand request, CancellationToken cancellationToken)
    {
        // Extract organization ID from JWT claims
        var orgIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("OrganizationId")?.Value;
        if (string.IsNullOrEmpty(orgIdClaim) || !Guid.TryParse(orgIdClaim, out var organizationId))
        {
            throw new UnauthorizedException("معرف المؤسسة مطلوب");
        }

        // Get the sale with tracking for update
        var sale = await _saleRepository.GetByIdWithTrackingAsync(request.SaleId, cancellationToken);
        if (sale == null)
        {
            throw new NotFoundException("جلسة البيع غير موجودة");
        }

        if (sale.OrganizationId != organizationId)
        {
            throw new UnauthorizedException("غير مصرح بالوصول إلى هذه الجلسة");
        }

        // Get product packaging by barcode or ID
        var packaging = request.ProductPackagingId.HasValue
            ? await _productPackagingRepository.GetByIdAsync(request.ProductPackagingId.Value, cancellationToken)
            : !string.IsNullOrWhiteSpace(request.Barcode)
                ? await _productPackagingRepository.GetByBarcodeAsync(request.Barcode, cancellationToken)
                : null;

        if (packaging == null)
        {
            throw new NotFoundException("المنتج غير موجود");
        }

        // Check stock availability
        var stock = await _stockRepository.GetByPackagingAsync(
            packaging.Id,
            organizationId,
            request.InventoryId,
            cancellationToken);

        if (stock == null || stock.TotalAvailableQuantity < request.Quantity)
        {
            var available = stock?.TotalAvailableQuantity ?? 0;
            throw new ValidationException($"الكمية المتاحة في المخزون ({available}) أقل من المطلوبة ({request.Quantity})");
        }

        // Get product name (need to load the product)
        var productName = packaging.Product?.Name ?? packaging.Name;

        // Add item to sale (uses existing domain method)
        await sale.AddItemAsync(
            productPackagingId: packaging.Id,
            productName: productName,
            quantity: request.Quantity,
            unitPrice: packaging.GetDiscountedPrice(),
            currencyService: _currencyService,
            discount: null, // Price already has discount applied
            cancellationToken: cancellationToken
        );

        await _saleRepository.SaveChangesAsync(cancellationToken);

        // Return the sale item ID (last added or updated)
        var addedItem = sale.Items.FirstOrDefault(i => i.ProductPackagingId == packaging.Id);
        return addedItem?.Id ?? sale.Id;
    }
}
