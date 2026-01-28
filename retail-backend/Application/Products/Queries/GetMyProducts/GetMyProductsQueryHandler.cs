using Application.Common.Services;
using Application.Products.DTOs;
using AutoMapper;
using Domains.Products.Repositories;
using Domains.Shared.Base;
using MediatR;

namespace Application.Products.Queries.GetMyProducts;

public class GetMyProductsQueryHandler : IRequestHandler<GetMyProductsQuery, PagedResult<ProductListDto>>
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

    public async Task<PagedResult<ProductListDto>> Handle(GetMyProductsQuery request, CancellationToken cancellationToken)
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

        // Get products without packagings
        var pagedProducts = await _productRepository.GetListAsync(
            organizationId,
            filter,
            pagingParams,
            cancellationToken);

        var dtos = _mapper.Map<List<ProductListDto>>(pagedProducts.Items);

        return new PagedResult<ProductListDto>(
            dtos,
            pagedProducts.TotalCount,
            pagedProducts.PageNumber,
            pagedProducts.PageSize);
    }
}
