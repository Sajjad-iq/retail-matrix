using Application.Common.Services;
using Application.Products.DTOs;
using AutoMapper;
using Domains.Products.Repositories;
using Domains.Shared.Base;
using MediatR;

namespace Application.Products.Queries.GetMyProducts;

public class GetMyProductsQueryHandler : IRequestHandler<GetMyProductsQuery, PagedResult<ProductWithPackagingsDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly IOrganizationContext _organizationContext;

    public GetMyProductsQueryHandler(
        IProductRepository productRepository,
        IMapper mapper,
        IOrganizationContext organizationContext)
    {
        _productRepository = productRepository;
        _mapper = mapper;
        _organizationContext = organizationContext;
    }

    public async Task<PagedResult<ProductWithPackagingsDto>> Handle(GetMyProductsQuery request, CancellationToken cancellationToken)
    {
        var organizationId = _organizationContext.OrganizationId;

        var pagingParams = new PagingParams
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };

        var filter = new Domains.Products.Models.ProductFilter
        {
            Ids = request.Ids,
            CategoryId = request.CategoryId,
            CategoryIds = request.CategoryIds,
            SearchTerm = request.SearchTerm,
            Status = request.Status,
            IsSelling = request.IsSelling,
            IsRawMaterial = request.IsRawMaterial
        };

        // Get products with their packagings
        var pagedProducts = await _productRepository.GetListAsync(
            organizationId,
            filter,
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
