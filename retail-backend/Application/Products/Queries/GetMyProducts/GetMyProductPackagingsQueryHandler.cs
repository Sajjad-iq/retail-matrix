using Application.Common.Exceptions;
using Application.Products.DTOs;
using AutoMapper;
using Domains.Products.Repositories;
using Domains.Shared.Base;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Application.Products.Queries.GetMyProductPackagings;

public class GetMyProductPackagingsQueryHandler : IRequestHandler<GetMyProductPackagingsQuery, PagedResult<ProductPackagingListDto>>
{
    private readonly IProductPackagingRepository _productPackagingRepository;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetMyProductPackagingsQueryHandler(
        IProductPackagingRepository productPackagingRepository,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor)
    {
        _productPackagingRepository = productPackagingRepository;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<PagedResult<ProductPackagingListDto>> Handle(GetMyProductPackagingsQuery request, CancellationToken cancellationToken)
    {
        // Get organization ID from claims
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

        var pagedPackagings = await _productPackagingRepository.GetByOrganizationAsync(
            organizationId,
            pagingParams,
            cancellationToken);

        var dtos = _mapper.Map<List<ProductPackagingListDto>>(pagedPackagings.Items);

        return new PagedResult<ProductPackagingListDto>(
            dtos,
            pagedPackagings.TotalCount,
            pagedPackagings.PageNumber,
            pagedPackagings.PageSize);
    }
}
