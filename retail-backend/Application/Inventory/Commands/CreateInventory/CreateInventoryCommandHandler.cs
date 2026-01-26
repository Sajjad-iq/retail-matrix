using Application.Common.Services;

using Domains.Inventory.Enums;
using Domains.Inventory.Repositories;
using MediatR;
using InventoryEntity = Domains.Inventory.Entities.Inventory;

namespace Application.Inventory.Commands.CreateInventory;

public class CreateInventoryCommandHandler : IRequestHandler<CreateInventoryCommand, Guid>
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IOrganizationContext _organizationContext;

    public CreateInventoryCommandHandler(IInventoryRepository inventoryRepository, IOrganizationContext organizationContext)
    {
        _inventoryRepository = inventoryRepository;
        _organizationContext = organizationContext;
    }

    public async Task<Guid> Handle(CreateInventoryCommand request, CancellationToken cancellationToken)
    {
        var organizationId = _organizationContext.OrganizationId;

        var inventory = InventoryEntity.Create(
            request.Name,
            request.Code,
            request.Type,
            organizationId,
            request.ParentId
        );

        await _inventoryRepository.AddAsync(inventory, cancellationToken);
        await _inventoryRepository.SaveChangesAsync(cancellationToken);

        return inventory.Id;
    }
}
