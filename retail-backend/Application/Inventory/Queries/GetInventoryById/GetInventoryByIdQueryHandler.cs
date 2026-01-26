using Application.Inventory.DTOs;
using AutoMapper;
using Domains.Inventory.Repositories;
using MediatR;

namespace Application.Inventory.Queries.GetInventoryById;

public class GetInventoryByIdQueryHandler : IRequestHandler<GetInventoryByIdQuery, InventoryDto>
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IMapper _mapper;

    public GetInventoryByIdQueryHandler(IInventoryRepository inventoryRepository, IMapper mapper)
    {
        _inventoryRepository = inventoryRepository;
        _mapper = mapper;
    }

    public async Task<InventoryDto> Handle(GetInventoryByIdQuery request, CancellationToken cancellationToken)
    {
        var inventory = await _inventoryRepository.GetByIdAsync(request.Id, cancellationToken);

        if (inventory == null)
            throw new KeyNotFoundException($"Inventory with ID {request.Id} not found.");

        return _mapper.Map<InventoryDto>(inventory);
    }
}
