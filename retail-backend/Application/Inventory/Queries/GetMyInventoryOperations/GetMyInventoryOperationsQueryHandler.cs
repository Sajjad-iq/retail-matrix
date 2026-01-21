using Application.Common.Exceptions;
using Application.Inventory.DTOs;
using AutoMapper;
using Domains.Inventory.Repositories;
using Domains.Shared.Base;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Application.Inventory.Queries.GetMyInventoryOperations;

public class GetMyInventoryOperationsQueryHandler : IRequestHandler<GetMyInventoryOperationsQuery, PagedResult<InventoryOperationListDto>>
{
    private readonly IInventoryOperationRepository _inventoryOperationRepository;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetMyInventoryOperationsQueryHandler(
        IInventoryOperationRepository inventoryOperationRepository,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor)
    {
        _inventoryOperationRepository = inventoryOperationRepository;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<PagedResult<InventoryOperationListDto>> Handle(GetMyInventoryOperationsQuery request, CancellationToken cancellationToken)
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

        var pagedOperations = await _inventoryOperationRepository.GetByOrganizationAsync(
            organizationId,
            pagingParams,
            cancellationToken);

        var dtos = _mapper.Map<List<InventoryOperationListDto>>(pagedOperations.Items);

        return new PagedResult<InventoryOperationListDto>(
            dtos,
            pagedOperations.TotalCount,
            pagedOperations.PageNumber,
            pagedOperations.PageSize);
    }
}
