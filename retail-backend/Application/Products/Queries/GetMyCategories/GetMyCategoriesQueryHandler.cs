using Application.Common.Exceptions;
using Application.Products.DTOs;
using AutoMapper;
using Domains.Products.Repositories;
using Domains.Shared.Base;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Products.Queries.GetMyCategories;

public class GetMyCategoriesQueryHandler : IRequestHandler<GetMyCategoriesQuery, PagedResult<CategoryDto>>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetMyCategoriesQueryHandler(
        ICategoryRepository categoryRepository,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<PagedResult<CategoryDto>> Handle(GetMyCategoriesQuery request, CancellationToken cancellationToken)
    {
        // Get organization ID from claims
        var orgIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("OrganizationId")?.Value;
        if (string.IsNullOrEmpty(orgIdClaim) || !Guid.TryParse(orgIdClaim, out var organizationId))
        {
            throw new UnauthorizedException("معرف المؤسسة مطلوب");
        }

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
