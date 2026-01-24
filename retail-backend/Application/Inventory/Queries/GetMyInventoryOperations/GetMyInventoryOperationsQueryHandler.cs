using Application.Common.Services;
using Application.Inventory.DTOs;
using AutoMapper;
using Domains.Inventory.Repositories;
using Domains.Shared.Base;
using MediatR;

namespace Application.Inventory.Queries.GetMyInventoryOperations;

public class GetMyInventoryOperationsQueryHandler : IRequestHandler<GetMyInventoryOperationsQuery, PagedResult<InventoryOperationListDto>>
{
    private readonly IInventoryOperationRepository _inventoryOperationRepository;
    private readonly IMapper _mapper;
    private readonly IOrganizationContext _organizationContext;

    public GetMyInventoryOperationsQueryHandler(
        IInventoryOperationRepository inventoryOperationRepository,
        IMapper mapper,
        IOrganizationContext organizationContext)
    {
        _inventoryOperationRepository = inventoryOperationRepository;
        _mapper = mapper;
        _organizationContext = organizationContext;
    }

    public async Task<PagedResult<InventoryOperationListDto>> Handle(GetMyInventoryOperationsQuery request, CancellationToken cancellationToken)
    {
        var organizationId = _organizationContext.OrganizationId;

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
