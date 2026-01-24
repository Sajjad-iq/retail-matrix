using Application.Common.Services;
using Application.Products.DTOs;
using AutoMapper;
using Domains.Products.Repositories;
using Domains.Shared.Base;
using MediatR;

namespace Application.Products.Queries.GetMyCategories;

public class GetMyCategoriesQueryHandler : IRequestHandler<GetMyCategoriesQuery, PagedResult<CategoryDto>>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;
    private readonly IOrganizationContext _organizationContext;

    public GetMyCategoriesQueryHandler(
        ICategoryRepository categoryRepository,
        IMapper mapper,
        IOrganizationContext organizationContext)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
        _organizationContext = organizationContext;
    }

    public async Task<PagedResult<CategoryDto>> Handle(GetMyCategoriesQuery request, CancellationToken cancellationToken)
    {
        var organizationId = _organizationContext.OrganizationId;

        // Get all categories for organization
        var categories = await _categoryRepository.GetByOrganizationAsync(organizationId, cancellationToken);
        var categoriesList = categories.ToList();

        // Manual pagination
        var totalCount = categoriesList.Count;
        var pagedItems = categoriesList
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var dtos = _mapper.Map<List<CategoryDto>>(pagedItems);

        return new PagedResult<CategoryDto>(
            dtos,
            totalCount,
            request.PageNumber,
            request.PageSize);
    }
}
