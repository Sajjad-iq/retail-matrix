using Domains.Inventory.Repositories;
using MediatR;

namespace Application.Inventory.Commands.UpdateInventory;

public class UpdateInventoryCommandHandler : IRequestHandler<UpdateInventoryCommand, bool>
{
    private readonly IInventoryRepository _inventoryRepository;

    public UpdateInventoryCommandHandler(IInventoryRepository inventoryRepository)
    {
        _inventoryRepository = inventoryRepository;
    }

    public async Task<bool> Handle(UpdateInventoryCommand request, CancellationToken cancellationToken)
    {
        var inventory = await _inventoryRepository.GetByIdAsync(request.Id, cancellationToken);

        if (inventory == null)
            throw new KeyNotFoundException($"Inventory with ID {request.Id} not found.");

        inventory.UpdateName(request.Name);
        inventory.UpdateCode(request.Code);
        inventory.UpdateType(request.Type);
        inventory.SetParent(request.ParentId);

        if (request.IsActive)
            inventory.Activate();
        else
            inventory.Deactivate();

        await _inventoryRepository.SaveChangesAsync(cancellationToken);

        return true;
    }
}
