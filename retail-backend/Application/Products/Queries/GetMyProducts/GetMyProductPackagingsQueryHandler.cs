using Application.Common.Services;
using Application.Products.DTOs;
using AutoMapper;
using Domains.Products.Repositories;
using Domains.Shared.Base;
using MediatR;

namespace Application.Products.Queries.GetMyProductPackagings;

public class GetMyProductPackagingsQueryHandler : IRequestHandler<GetMyProductPackagingsQuery, PagedResult<ProductPackagingListDto>>
{
    private readonly IProductPackagingRepository _productPackagingRepository;
    private readonly IMapper _mapper;
    private readonly IOrganizationContext _organizationContext;

    public GetMyProductPackagingsQueryHandler(
        IProductPackagingRepository productPackagingRepository,
        IMapper mapper,
        IOrganizationContext organizationContext)
    {
        _productPackagingRepository = productPackagingRepository;
        _mapper = mapper;
        _organizationContext = organizationContext;
    }

    public async Task<PagedResult<ProductPackagingListDto>> Handle(GetMyProductPackagingsQuery request, CancellationToken cancellationToken)
    {
        var organizationId = _organizationContext.OrganizationId;

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
