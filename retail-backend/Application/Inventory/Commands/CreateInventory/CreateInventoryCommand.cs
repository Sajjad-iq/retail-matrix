using Domains.Inventory.Enums;
using MediatR;

namespace Application.Inventory.Commands.CreateInventory;

public record CreateInventoryCommand(string Name, string Code, InventoryType Type, Guid? ParentId) : IRequest<Guid>;
