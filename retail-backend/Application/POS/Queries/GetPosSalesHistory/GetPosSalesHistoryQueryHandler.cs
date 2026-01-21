using Application.Common.Exceptions;
using Application.POS.DTOs;
using Domains.Sales.Repositories;
using Domains.Shared.Base;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.POS.Queries.GetPosSalesHistory;

public class GetPosSalesHistoryQueryHandler : IRequestHandler<GetPosSalesHistoryQuery, PagedResult<PosSaleListDto>>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetPosSalesHistoryQueryHandler(
        ISaleRepository saleRepository,
        IHttpContextAccessor httpContextAccessor)
    {
        _saleRepository = saleRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<PagedResult<PosSaleListDto>> Handle(GetPosSalesHistoryQuery request, CancellationToken cancellationToken)
    {
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

        // Get sales based on filters
        PagedResult<Domains.Sales.Entities.Sale> result;

        if (request.Status.HasValue)
        {
            result = await _saleRepository.GetByStatusAsync(
                organizationId,
                request.Status.Value,
                pagingParams,
                cancellationToken);
        }
        else if (request.StartDate.HasValue && request.EndDate.HasValue)
        {
            result = await _saleRepository.GetByDateRangeAsync(
                organizationId,
                request.StartDate.Value,
                request.EndDate.Value,
                pagingParams,
                cancellationToken);
        }
        else
        {
            result = await _saleRepository.GetByOrganizationAsync(
                organizationId,
                pagingParams,
                cancellationToken);
        }

        // Map to DTOs
        var dtos = result.Items.Select(s => new PosSaleListDto
        {
            SaleId = s.Id,
            SaleNumber = s.SaleNumber,
            SaleDate = s.SaleDate,
            Status = s.Status,
            GrandTotal = s.GrandTotal,
            ItemCount = s.Items.Count
        }).ToList();

        return new PagedResult<PosSaleListDto>(
            dtos,
            result.TotalCount,
            result.PageNumber,
            result.PageSize);
    }
}
