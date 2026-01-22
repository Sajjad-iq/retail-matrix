using Application.Common.Exceptions;
using Domains.Common.Currency.Services;
using Domains.Products.Repositories;
using Domains.Sales.Entities;
using Domains.Sales.Repositories;
using Domains.Shared.ValueObjects;
using Domains.Stocks.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.POS.Commands.CreateSale;

public class CreateSaleCommandHandler : IRequestHandler<CreateSaleCommand, Guid>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IProductPackagingRepository _productPackagingRepository;
    private readonly IStockRepository _stockRepository;
    private readonly ICurrencyConversionService _currencyService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CreateSaleCommandHandler(
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

    public async Task<Guid> Handle(CreateSaleCommand request, CancellationToken cancellationToken)
    {
        // Extract organization ID and user ID from JWT claims
        var orgIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("OrganizationId")?.Value;
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("UserId")?.Value;

        if (string.IsNullOrEmpty(orgIdClaim) || !Guid.TryParse(orgIdClaim, out var organizationId))
        {
            throw new UnauthorizedException("معرف المؤسسة مطلوب");
        }

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedException("معرف المستخدم مطلوب");
        }

        // Create a new draft sale
        var sale = Sale.Create(
            organizationId: organizationId,
            salesPersonId: userId
        );

        // Add notes if provided
        if (!string.IsNullOrWhiteSpace(request.Notes))
        {
            sale.AddNotes(request.Notes);
        }

        // Add items if provided
        foreach (var itemInput in request.Items)
        {
            await AddItemToSaleAsync(sale, itemInput, organizationId, request.InventoryId, cancellationToken);
        }

        // Persist the sale
        await _saleRepository.AddAsync(sale, cancellationToken);
        await _saleRepository.SaveChangesAsync(cancellationToken);

        return sale.Id;
    }

    private async Task AddItemToSaleAsync(
        Sale sale,
        SaleItemInput itemInput,
        Guid organizationId,
        Guid inventoryId,
        CancellationToken cancellationToken)
    {
        // Get product packaging by barcode or ID
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
            inventoryId,
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
}
