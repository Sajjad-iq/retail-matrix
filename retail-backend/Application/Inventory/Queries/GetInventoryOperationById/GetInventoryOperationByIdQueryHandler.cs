using Application.Common.Exceptions;
using Application.Inventory.DTOs;
using AutoMapper;
using Domains.Inventory.Repositories;
using MediatR;

namespace Application.Inventory.Queries.GetInventoryOperationById;

public class GetInventoryOperationByIdQueryHandler : IRequestHandler<GetInventoryOperationByIdQuery, InventoryOperationDto>
{
    private readonly IInventoryOperationRepository _inventoryOperationRepository;
    private readonly IMapper _mapper;

    public GetInventoryOperationByIdQueryHandler(
        IInventoryOperationRepository inventoryOperationRepository,
        IMapper mapper)
    {
        _inventoryOperationRepository = inventoryOperationRepository;
        _mapper = mapper;
    }

    public async Task<InventoryOperationDto> Handle(GetInventoryOperationByIdQuery request, CancellationToken cancellationToken)
    {
        var operation = await _inventoryOperationRepository.GetByIdAsync(request.OperationId, cancellationToken);

        if (operation == null)
        {
            throw new NotFoundException("عملية المخزون غير موجودة");
        }

        return _mapper.Map<InventoryOperationDto>(operation);
    }
}
