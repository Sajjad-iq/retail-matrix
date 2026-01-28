using Application.Inventory.DTOs;
using Domains.Shared.Base;
using MediatR;

namespace Application.Inventory.Queries.GetMyInventoryOperations;

public record GetMyInventoryOperationsQuery : IRequest<PagedResult<InventoryOperationListDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;

    public string? SearchTerm { get; init; }
    public Domains.Inventory.Enums.InventoryOperationStatus? Status { get; init; }
    public Domains.Inventory.Enums.InventoryOperationType? Type { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public Guid? SourceInventoryId { get; init; }
    public Guid? DestinationInventoryId { get; init; }
}
