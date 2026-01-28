using Application.Inventory.DTOs;
using AutoMapper;
using Domains.Inventory.Repositories;
using MediatR;

namespace Application.Inventory.Queries.GetInventoryOperationItems;

public class GetInventoryOperationItemsQueryHandler : IRequestHandler<GetInventoryOperationItemsQuery, List<InventoryOperationItemDto>>
{
    private readonly IInventoryOperationRepository _repository;
    private readonly IMapper _mapper;

    public GetInventoryOperationItemsQueryHandler(
        IInventoryOperationRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<InventoryOperationItemDto>> Handle(GetInventoryOperationItemsQuery request, CancellationToken cancellationToken)
    {
        var operation = await _repository.GetByIdAsync(request.OperationId, cancellationToken);

        if (operation == null)
        {
            return new List<InventoryOperationItemDto>();
        }

        return _mapper.Map<List<InventoryOperationItemDto>>(operation.Items);
    }
}
