using Application.Common.Exceptions;
using Application.Inventory.DTOs;
using AutoMapper;
using Domains.Inventory.Repositories;
using Domains.Shared.Base;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using InventoryEntity = Domains.Inventory.Entities.Inventory;

namespace Application.Inventory.Queries.GetMyInventories;

public class GetMyInventoriesQueryHandler : IRequestHandler<GetMyInventoriesQuery, PagedResult<InventoryDto>>
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetMyInventoriesQueryHandler(
        IInventoryRepository inventoryRepository,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor)
    {
        _inventoryRepository = inventoryRepository;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<PagedResult<InventoryDto>> Handle(GetMyInventoriesQuery request, CancellationToken cancellationToken)
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

        var pagedInventories = await _inventoryRepository.GetByOrganizationAsync(
            organizationId,
            pagingParams,
            cancellationToken);

        var dtos = _mapper.Map<List<InventoryDto>>(pagedInventories.Items);

        return new PagedResult<InventoryDto>(
            dtos,
            pagedInventories.TotalCount,
            pagedInventories.PageNumber,
            pagedInventories.PageSize);
    }
}
