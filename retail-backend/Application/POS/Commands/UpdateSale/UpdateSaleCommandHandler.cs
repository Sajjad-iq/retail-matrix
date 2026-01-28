using Application.Common.Exceptions;
using Application.Common.Services;
using Domains.Common.Currency.Services;
using Domains.Products.Repositories;
using Domains.Sales.Repositories;
using Domains.Shared.ValueObjects;
using Domains.Stocks.Repositories;
using MediatR;

namespace Application.POS.Commands.UpdateSale;

public class UpdateSaleCommandHandler : IRequestHandler<UpdateSaleCommand, bool>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IProductPackagingRepository _productPackagingRepository;
    private readonly IStockRepository _stockRepository;
    private readonly ICurrencyConversionService _currencyService;
    private readonly IOrganizationContext _organizationContext;

    public UpdateSaleCommandHandler(
        ISaleRepository saleRepository,
        IProductPackagingRepository productPackagingRepository,
        IStockRepository stockRepository,
        ICurrencyConversionService currencyService,
        IOrganizationContext organizationContext)
    {
        _saleRepository = saleRepository;
        _productPackagingRepository = productPackagingRepository;
        _stockRepository = stockRepository;
        _currencyService = currencyService;
        _organizationContext = organizationContext;
    }

    public async Task<bool> Handle(UpdateSaleCommand request, CancellationToken cancellationToken)
    {
        var organizationId = _organizationContext.OrganizationId;

        // Get the sale with tracking for update
        var sale = await _saleRepository.GetByIdWithTrackingAsync(request.SaleId, cancellationToken);
        if (sale == null)
        {
            throw new NotFoundException("البيع غير موجود");
        }

        if (sale.OrganizationId != organizationId)
        {
            throw new UnauthorizedException("غير مصرح بالوصول إلى هذا البيع");
        }

        // Remove all existing items
        // TODO: Optimize this by diffing and only updating changed items
        // Current approach: Remove all, then add all (simple but inefficient)
        var itemsToRemove = sale.Items.ToList();
        foreach (var item in itemsToRemove)
        {
            await sale.RemoveItemAsync(item.Id, _currencyService, cancellationToken);
        }

        // Add new items
        foreach (var itemInput in request.Items)
        {
            // Get product packaging
            var packaging = itemInput.ProductPackagingId.HasValue
                ? await _productPackagingRepository.GetByIdAsync(itemInput.ProductPackagingId.Value, cancellationToken)
                : !string.IsNullOrWhiteSpace(itemInput.Barcode)
                    ? await _productPackagingRepository.GetByBarcodeAsync(itemInput.Barcode, cancellationToken)
                    : null;

            if (packaging == null)
            {
                var identifier = itemInput.Barcode ?? itemInput.ProductPackagingId?.ToString() ?? "غير معروف";
                throw new NotFoundException($"المنتج غير موجود: {identifier}");
            }

            // Check stock availability
            var stock = await _stockRepository.GetByPackagingAsync(
                packaging.Id,
                organizationId,
                request.InventoryId,
                cancellationToken);

            if (stock == null || stock.TotalAvailableQuantity < itemInput.Quantity)
            {
                var available = stock?.TotalAvailableQuantity ?? 0;
                throw new ValidationException($"الكمية المتاحة في المخزون ({available}) أقل من المطلوبة ({itemInput.Quantity}) للمنتج: {packaging.Name}");
            }

            // Get product name
            var productName = packaging.Product?.Name ?? packaging.Name;

            // Calculate discount if provided
            Discount? discount = null;
            if (itemInput.Discount != null)
            {
                discount = itemInput.Discount.IsPercentage
                    ? Discount.Percentage(itemInput.Discount.Amount)
                    : Discount.FixedAmount(itemInput.Discount.Amount);
            }

            // Add item to sale
            await sale.AddItemAsync(
                productPackagingId: packaging.Id,
                productName: productName,
                quantity: itemInput.Quantity,
                unitPrice: packaging.GetDiscountedPrice(),
                currencyService: _currencyService,
                discount: discount,
                cancellationToken: cancellationToken
            );
        }

        // Update notes if provided
        if (request.Notes != null)
        {
            sale.AddNotes(request.Notes);
        }

        await _saleRepository.SaveChangesAsync(cancellationToken);

        return true;
    }
}
