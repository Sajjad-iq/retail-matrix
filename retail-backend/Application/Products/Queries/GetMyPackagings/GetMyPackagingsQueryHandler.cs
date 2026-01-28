using Application.Common.Services;
using Application.Products.DTOs;
using AutoMapper;
using Domains.Shared.Base;
using Domains.Products.Repositories;
using MediatR;

namespace Application.Products.Queries.GetMyPackagings;

public class GetMyPackagingsQueryHandler : IRequestHandler<GetMyPackagingsQuery, PagedResult<ProductPackagingListDto>>
{
    private readonly IProductPackagingRepository _packagingRepository;
    private readonly IMapper _mapper;
    private readonly IOrganizationContext _organizationContext;

    public GetMyPackagingsQueryHandler(
        IProductPackagingRepository packagingRepository,
        IMapper mapper,
        IOrganizationContext organizationContext)
    {
        _packagingRepository = packagingRepository;
        _mapper = mapper;
        _organizationContext = organizationContext;
    }

    public async Task<PagedResult<ProductPackagingListDto>> Handle(GetMyPackagingsQuery request, CancellationToken cancellationToken)
    {
        // 1. Get organization ID from context
        var organizationId = _organizationContext.OrganizationId;

        var pagingParams = new PagingParams
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };

        // 2. Query using repository method with filter
        var result = await _packagingRepository.GetListAsync(
            organizationId,
            new Domains.Products.Models.ProductPackagingFilter
            {
                ProductId = request.ProductId,
                Status = request.Status,
                SearchTerm = request.SearchTerm,
                Barcode = request.Barcode,
                IsDefault = request.IsDefault
            },
            pagingParams,
            cancellationToken);

        // 3. Map to DTOs
        var dtos = _mapper.Map<List<ProductPackagingListDto>>(result.Items);

        return new PagedResult<ProductPackagingListDto>(
            dtos,
            result.TotalCount,
            result.PageNumber,
            result.PageSize);
    }
}
