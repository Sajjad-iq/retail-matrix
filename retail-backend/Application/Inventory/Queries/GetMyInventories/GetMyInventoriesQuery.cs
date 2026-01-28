using Application.Inventory.DTOs;
using Domains.Shared.Base;
using MediatR;

namespace Application.Inventory.Queries.GetMyInventories;

public record GetMyInventoriesQuery : IRequest<PagedResult<InventoryDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;

    public string? SearchTerm { get; init; }
    public Domains.Inventory.Enums.InventoryType? Type { get; init; }
    public Guid? ParentId { get; init; }
    public bool? IsActive { get; init; }
}
