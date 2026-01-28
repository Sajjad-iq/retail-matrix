using Application.Common.Exceptions;
using Application.Common.Services;
using Application.Stocks.DTOs;
using AutoMapper;
using Domains.Shared.Base;
using Domains.Stocks.Repositories;
using MediatR;

namespace Application.Stocks.Queries.GetMyStocks;

public class GetMyStocksQueryHandler : IRequestHandler<GetMyStocksQuery, PagedResult<StockListDto>>
{
    private readonly IStockRepository _stockRepository;
    private readonly IMapper _mapper;
    private readonly IOrganizationContext _organizationContext;

    public GetMyStocksQueryHandler(
        IStockRepository stockRepository,
        IMapper mapper,
        IOrganizationContext organizationContext)
    {
        _stockRepository = stockRepository;
        _mapper = mapper;
        _organizationContext = organizationContext;
    }

    public async Task<PagedResult<StockListDto>> Handle(GetMyStocksQuery request, CancellationToken cancellationToken)
    {
        // 1. Get organization ID from context
        var organizationId = _organizationContext.OrganizationId;

        // 2. Validate low stock filter
        if (request.StockStatus == StockStatus.LowStock && !request.ReorderLevel.HasValue)
        {
            throw new ValidationException("مستوى إعادة الطلب مطلوب لفلترة المخزون المنخفض");
        }

        var pagingParams = new PagingParams
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };

        // 3. Query using single repository method with all filters
        var result = await _stockRepository.GetByFiltersAsync(
            organizationId: organizationId,
            inventoryId: request.InventoryId,
            productId: request.ProductId,
            productPackagingId: request.ProductPackagingId,
            productName: request.ProductName,
            isLowStock: request.StockStatus == StockStatus.LowStock,
            reorderLevel: request.ReorderLevel,
            isOutOfStock: request.StockStatus == StockStatus.OutOfStock,
            pagingParams: pagingParams,
            cancellationToken: cancellationToken);

        // 4. Map to DTOs
        var dtos = _mapper.Map<List<StockListDto>>(result.Items);

        return new PagedResult<StockListDto>(
            dtos,
            result.TotalCount,
            result.PageNumber,
            result.PageSize);
    }
}
