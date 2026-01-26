using Application.Inventory.DTOs;
using MediatR;

namespace Application.Inventory.Queries.GetInventoryById;

public record GetInventoryByIdQuery(Guid Id) : IRequest<InventoryDto>;
