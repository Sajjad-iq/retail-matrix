using Application.POS.DTOs;
using Domains.Sales.Enums;
using Domains.Shared.Base;
using MediatR;

namespace Application.POS.Queries.GetPosSalesHistory;

/// <summary>
/// Query to get paginated sales history
/// </summary>
public record GetPosSalesHistoryQuery : IRequest<PagedResult<PosSaleListDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public SaleStatus? Status { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
}
