using Application.Common.Exceptions;
using Application.POS.DTOs;
using Domains.Products.Repositories;
using Domains.Stocks.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.POS.Queries.SearchProductByBarcode;

public class SearchProductByBarcodeQueryHandler : IRequestHandler<SearchProductByBarcodeQuery, PosProductDto?>
{
    private readonly IProductPackagingRepository _productPackagingRepository;
    private readonly IStockRepository _stockRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SearchProductByBarcodeQueryHandler(
        IProductPackagingRepository productPackagingRepository,
        IStockRepository stockRepository,
        IHttpContextAccessor httpContextAccessor)
    {
        _productPackagingRepository = productPackagingRepository;
        _stockRepository = stockRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<PosProductDto?> Handle(SearchProductByBarcodeQuery request, CancellationToken cancellationToken)
    {
        var orgIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("OrganizationId")?.Value;
        if (string.IsNullOrEmpty(orgIdClaim) || !Guid.TryParse(orgIdClaim, out var organizationId))
        {
            throw new UnauthorizedException("معرف المؤسسة مطلوب");
        }

        // Find product packaging by barcode
        var packaging = await _productPackagingRepository.GetByBarcodeAsync(request.Barcode, cancellationToken);
        if (packaging == null)
        {
            return null;
        }

        // Get stock availability
        var stock = await _stockRepository.GetByPackagingAsync(
            packaging.Id,
            organizationId,
            request.InventoryId,
            cancellationToken);

        var availableQuantity = stock?.TotalAvailableQuantity ?? 0;

        return new PosProductDto
        {
            ProductId = packaging.ProductId,
            ProductName = packaging.Product?.Name ?? packaging.Name,
            PackagingId = packaging.Id,
            PackagingName = packaging.Name,
            Barcode = packaging.Barcode?.ToString(),
            SellingPrice = packaging.SellingPrice,
            Discount = packaging.Discount,
            DiscountedPrice = packaging.GetDiscountedPrice(),
            AvailableStock = availableQuantity
        };
    }
}
