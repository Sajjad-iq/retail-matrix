using Domains.Inventory.Enums;
using MediatR;

namespace Application.Inventory.Commands.UpdateInventory;

public record UpdateInventoryCommand(Guid Id, string Name, string Code, InventoryType Type, Guid? ParentId, bool IsActive) : IRequest<bool>;
