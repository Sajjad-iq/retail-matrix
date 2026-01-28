using Application.Common.Services;
using Application.Inventory.DTOs;
using AutoMapper;
using Domains.Inventory.Repositories;
using Domains.Shared.Base;
using MediatR;
using InventoryEntity = Domains.Inventory.Entities.Inventory;

namespace Application.Inventory.Queries.GetMyInventories;

public class GetMyInventoriesQueryHandler : IRequestHandler<GetMyInventoriesQuery, PagedResult<InventoryDto>>
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IMapper _mapper;
    private readonly IOrganizationContext _organizationContext;

    public GetMyInventoriesQueryHandler(
        IInventoryRepository inventoryRepository,
        IMapper mapper,
        IOrganizationContext organizationContext)
    {
        _inventoryRepository = inventoryRepository;
        _mapper = mapper;
        _organizationContext = organizationContext;
    }

    public async Task<PagedResult<InventoryDto>> Handle(GetMyInventoriesQuery request, CancellationToken cancellationToken)
    {
        var organizationId = _organizationContext.OrganizationId;

        var pagingParams = new PagingParams
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };

        var filter = new Domains.Inventory.Models.InventoryFilter
        {
            SearchTerm = request.SearchTerm,
            Type = request.Type,
            ParentId = request.ParentId,
            IsActive = request.IsActive
        };

        var pagedInventories = await _inventoryRepository.GetListAsync(
            organizationId,
            filter,
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
