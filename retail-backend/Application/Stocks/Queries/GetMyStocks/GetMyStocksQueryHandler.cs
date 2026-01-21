using Application.Common.Exceptions;
using Application.Stocks.DTOs;
using AutoMapper;
using Domains.Shared.Base;
using Domains.Stocks.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Stocks.Queries.GetMyStocks;

public class GetMyStocksQueryHandler : IRequestHandler<GetMyStocksQuery, PagedResult<StockListDto>>
{
    private readonly IStockRepository _stockRepository;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetMyStocksQueryHandler(
        IStockRepository stockRepository,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor)
    {
        _stockRepository = stockRepository;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<PagedResult<StockListDto>> Handle(GetMyStocksQuery request, CancellationToken cancellationToken)
    {
        // 1. Get organization ID from claims
        var orgIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("OrganizationId")?.Value;
        if (string.IsNullOrEmpty(orgIdClaim) || !Guid.TryParse(orgIdClaim, out var organizationId))
        {
            throw new UnauthorizedException("معرف المؤسسة مطلوب");
        }

        var pagingParams = new PagingParams
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };

        // 2. Query based on status filter
        PagedResult<Domains.Stocks.Entities.Stock> result;

        switch (request.StockStatus)
        {
            case StockStatus.LowStock:
                if (!request.ReorderLevel.HasValue)
                {
                    throw new ValidationException("مستوى إعادة الطلب مطلوب لفلترة المخزون المنخفض");
                }
                result = await _stockRepository.GetLowStockItemsAsync(
                    organizationId,
                    request.ReorderLevel.Value,
                    pagingParams,
                    cancellationToken);
                break;

            case StockStatus.OutOfStock:
                result = await _stockRepository.GetOutOfStockItemsAsync(
                    organizationId,
                    pagingParams,
                    cancellationToken);
                break;

            case StockStatus.All:
            default:
                if (request.InventoryId.HasValue)
                {
                    result = await _stockRepository.GetByInventoryAsync(
                        request.InventoryId.Value,
                        pagingParams,
                        cancellationToken);
                }
                else
                {
                    result = await _stockRepository.GetByOrganizationAsync(
                        organizationId,
                        pagingParams,
                        cancellationToken);
                }
                break;
        }

        // 3. Map to DTOs
        var dtos = _mapper.Map<List<StockListDto>>(result.Items);

        return new PagedResult<StockListDto>(
            dtos,
            result.TotalCount,
            result.PageNumber,
            result.PageSize);
    }
}
