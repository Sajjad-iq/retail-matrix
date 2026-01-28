using Application.Common.Services;
using Application.POS.DTOs;
using Domains.Products.Repositories;
using Domains.Shared.Base;
using Domains.Stocks.Repositories;
using MediatR;

namespace Application.POS.Queries.GetInventoryProducts;

public class GetInventoryProductsQueryHandler : IRequestHandler<GetInventoryProductsQuery, PagedResult<PosProductDto>>
{
    private readonly IProductPackagingRepository _productPackagingRepository;
    private readonly IStockRepository _stockRepository;
    private readonly IOrganizationContext _organizationContext;

    public GetInventoryProductsQueryHandler(
        IProductPackagingRepository productPackagingRepository,
        IStockRepository stockRepository,
        IOrganizationContext organizationContext)
    {
        _productPackagingRepository = productPackagingRepository;
        _stockRepository = stockRepository;
        _organizationContext = organizationContext;
    }

    public async Task<PagedResult<PosProductDto>> Handle(GetInventoryProductsQuery request, CancellationToken cancellationToken)
    {
        // 1. Get organization ID from context
        var organizationId = _organizationContext.OrganizationId;

        var pagingParams = new PagingParams
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };

        // 2. Build filter for product packagings
        var filter = new Domains.Products.Models.ProductPackagingFilter
        {
            Status = request.Status,
            SearchTerm = request.SearchTerm,
            Barcode = request.Barcode
        };

        // 3. Get paginated product packagings
        var pagedPackagings = await _productPackagingRepository.GetListAsync(
            organizationId,
            filter,
            pagingParams,
            cancellationToken);

        // 4. Group packagings by product and build PosProductDto with packaging list
        var productGroups = pagedPackagings.Items
            .GroupBy(p => p.ProductId)
            .ToList();

        var productDtos = new List<PosProductDto>();

        foreach (var productGroup in productGroups)
        {
            var firstPackaging = productGroup.First();
            
            // Filter by category if specified
            if (request.CategoryId.HasValue && firstPackaging.Product?.CategoryId != request.CategoryId.Value)
            {
                continue;
            }

            // Build packaging DTOs with stock information
            var packagingDtos = new List<PosPackagingDto>();

            foreach (var packaging in productGroup)
            {
                // Get stock for this packaging in the specified inventory
                var stock = await _stockRepository.GetByPackagingAsync(
                    packaging.Id,
                    organizationId,
                    request.InventoryId,
                    cancellationToken);

                var availableQuantity = stock?.TotalAvailableQuantity ?? 0;

                // Filter by stock availability if specified
                if (request.InStock.HasValue)
                {
                    if (request.InStock.Value && availableQuantity <= 0)
                        continue;
                    if (!request.InStock.Value && availableQuantity > 0)
                        continue;
                }

                // Filter by minimum quantity if specified
                if (request.MinQuantity.HasValue && availableQuantity < request.MinQuantity.Value)
                {
                    continue;
                }

                var packagingDto = new PosPackagingDto
                {
                    PackagingId = packaging.Id,
                    PackagingName = packaging.Name,
                    Barcode = packaging.Barcode?.ToString(),
                    IsDefault = packaging.IsDefault,
                    SellingPrice = packaging.SellingPrice,
                    DiscountedPrice = packaging.GetDiscountedPrice(),
                    AvailableStock = availableQuantity
                };

                packagingDtos.Add(packagingDto);
            }

            // Skip product if no packagings passed the filters
            if (packagingDtos.Count == 0)
            {
                continue;
            }

            var productDto = new PosProductDto
            {
                ProductId = firstPackaging.ProductId,
                ProductName = firstPackaging.Product?.Name ?? firstPackaging.Name,
                Packagings = packagingDtos
            };

            productDtos.Add(productDto);
        }

        // 5. Return paginated result
        // Note: Since we're grouping by product, the count represents packagings not products
        return new PagedResult<PosProductDto>(
            productDtos,
            productGroups.Count, // Number of unique products
            request.PageNumber,
            request.PageSize);
    }
}
