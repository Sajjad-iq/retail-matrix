using Application.Common.Exceptions;
using Application.Products.DTOs;
using AutoMapper;
using Domains.Products.Repositories;
using Domains.Shared.Base;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Application.Products.Queries.GetMyProducts;

public class GetMyProductsQueryHandler : IRequestHandler<GetMyProductsQuery, PagedResult<ProductWithPackagingsDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetMyProductsQueryHandler(
        IProductRepository productRepository,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor)
    {
        _productRepository = productRepository;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<PagedResult<ProductWithPackagingsDto>> Handle(GetMyProductsQuery request, CancellationToken cancellationToken)
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

        // Get products with their packagings
        var pagedProducts = await _productRepository.GetByOrganizationAsync(
            organizationId,
            pagingParams,
            cancellationToken);

        var dtos = _mapper.Map<List<ProductWithPackagingsDto>>(pagedProducts.Items);

        return new PagedResult<ProductWithPackagingsDto>(
            dtos,
            pagedProducts.TotalCount,
            pagedProducts.PageNumber,
            pagedProducts.PageSize);
    }
}
