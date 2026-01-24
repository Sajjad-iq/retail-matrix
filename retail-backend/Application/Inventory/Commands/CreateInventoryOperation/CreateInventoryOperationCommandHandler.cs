using Application.Common.Services;
using Domains.Inventory.Entities;
using Domains.Inventory.Repositories;
using MediatR;

namespace Application.Inventory.Commands.CreateInventoryOperation;

public class CreateInventoryOperationCommandHandler : IRequestHandler<CreateInventoryOperationCommand, Guid>
{
    private readonly IInventoryOperationRepository _inventoryOperationRepository;
    private readonly IOrganizationContext _organizationContext;

    public CreateInventoryOperationCommandHandler(
        IInventoryOperationRepository inventoryOperationRepository,
        IOrganizationContext organizationContext)
    {
        _inventoryOperationRepository = inventoryOperationRepository;
        _organizationContext = organizationContext;
    }

    public async Task<Guid> Handle(CreateInventoryOperationCommand request, CancellationToken cancellationToken)
    {
        // 1. Get current user ID and organization ID from context
        var userId = _organizationContext.UserId;
        var organizationId = _organizationContext.OrganizationId;

        // 2. Create inventory operation using factory method
        var operation = InventoryOperation.Create(
            operationType: request.OperationType,
            operationNumber: request.OperationNumber,
            userId: userId,
            organizationId: organizationId,
            sourceInventoryId: request.SourceInventoryId,
            destinationInventoryId: request.DestinationInventoryId,
            notes: request.Notes
        );

        // 3. Persist operation
        await _inventoryOperationRepository.AddAsync(operation, cancellationToken);
        await _inventoryOperationRepository.SaveChangesAsync(cancellationToken);

        // 4. Return operation ID
        return operation.Id;
    }
}
