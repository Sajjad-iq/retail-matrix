using Application.Common.Services;
using Application.Stocks.DTOs;
using AutoMapper;
using Domains.Shared.Base;
using Domains.Stocks.Entities;
using Domains.Stocks.Repositories;
using Domains.Stocks.Models;
using MediatR;

namespace Application.Stocks.Queries.GetMyBatches;

public class GetMyBatchesQueryHandler : IRequestHandler<GetMyBatchesQuery, PagedResult<StockBatchDto>>
{
    private readonly IStockRepository _stockRepository;
    private readonly IMapper _mapper;
    private readonly IOrganizationContext _organizationContext;

    public GetMyBatchesQueryHandler(
        IStockRepository stockRepository,
        IMapper mapper,
        IOrganizationContext organizationContext)
    {
        _stockRepository = stockRepository;
        _mapper = mapper;
        _organizationContext = organizationContext;
    }

    public async Task<PagedResult<StockBatchDto>> Handle(GetMyBatchesQuery request, CancellationToken cancellationToken)
    {
        // 1. Get organization ID from context
        var organizationId = _organizationContext.OrganizationId;

        var pagingParams = new PagingParams
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };

        // 2. Query based on batch status and condition filter
        var filter = new StockBatchFilter();

        if (request.Condition.HasValue)
        {
            filter = filter with { Condition = request.Condition.Value };
        }
        else
        {
            switch (request.BatchStatus)
            {
                case BatchStatus.Expired:
                    filter = filter with { IsExpired = true };
                    break;

                case BatchStatus.NearExpiry:
                    filter = filter with { DaysToExpiry = request.DaysThreshold };
                    break;

                case BatchStatus.All:
                default:
                    // For "All" batches, we get all batches with expiry dates
                    filter = filter with { DaysToExpiry = int.MaxValue };
                    break;
            }
        }

        var result = await _stockRepository.GetBatchesListAsync(
            organizationId,
            filter,
            pagingParams,
            cancellationToken);

        // 3. Map to DTOs
        var dtos = _mapper.Map<List<StockBatchDto>>(result.Items);

        return new PagedResult<StockBatchDto>(
            dtos,
            result.TotalCount,
            result.PageNumber,
            result.PageSize);
    }
}
