using Application.Common.Exceptions;
using Application.Stocks.DTOs;
using AutoMapper;
using Domains.Shared.Base;
using Domains.Stocks.Entities;
using Domains.Stocks.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Stocks.Queries.GetMyBatches;

public class GetMyBatchesQueryHandler : IRequestHandler<GetMyBatchesQuery, PagedResult<StockBatchDto>>
{
    private readonly IStockRepository _stockRepository;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetMyBatchesQueryHandler(
        IStockRepository stockRepository,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor)
    {
        _stockRepository = stockRepository;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<PagedResult<StockBatchDto>> Handle(GetMyBatchesQuery request, CancellationToken cancellationToken)
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

        // 2. Query based on batch status and condition filter
        PagedResult<StockBatch> result;

        // If condition filter is specified, use condition-based query
        if (request.Condition.HasValue)
        {
            result = await _stockRepository.GetBatchesByConditionAsync(
                organizationId,
                request.Condition.Value,
                pagingParams,
                cancellationToken);
        }
        else
        {
            // Query based on batch status
            switch (request.BatchStatus)
            {
                case BatchStatus.Expired:
                    result = await _stockRepository.GetExpiredBatchesAsync(
                        organizationId,
                        pagingParams,
                        cancellationToken);
                    break;

                case BatchStatus.NearExpiry:
                    result = await _stockRepository.GetNearExpiryBatchesAsync(
                        organizationId,
                        request.DaysThreshold,
                        pagingParams,
                        cancellationToken);
                    break;

                case BatchStatus.All:
                default:
                    // For "All" batches, we need to get expired batches as a default
                    // since the repository doesn't have a GetAllBatches method
                    // We'll use a large threshold to get all batches with expiry dates
                    result = await _stockRepository.GetNearExpiryBatchesAsync(
                        organizationId,
                        int.MaxValue,
                        pagingParams,
                        cancellationToken);
                    break;
            }
        }

        // 3. Map to DTOs
        var dtos = _mapper.Map<List<StockBatchDto>>(result.Items);

        return new PagedResult<StockBatchDto>(
            dtos,
            result.TotalCount,
            result.PageNumber,
            result.PageSize);
    }
}
