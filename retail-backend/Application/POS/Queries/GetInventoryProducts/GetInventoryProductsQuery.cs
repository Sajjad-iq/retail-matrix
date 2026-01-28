using Application.POS.DTOs;
using Domains.Products.Enums;
using Domains.Shared.Base;
using MediatR;

namespace Application.POS.Queries.GetInventoryProducts;

/// <summary>
/// Query to get paginated products with stock availability for a specific inventory
/// </summary>
public record GetInventoryProductsQuery : IRequest<PagedResult<PosProductDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public Guid InventoryId { get; init; }
    public Guid? CategoryId { get; init; }
    public string? SearchTerm { get; init; }
    public string? Barcode { get; init; }
    public ProductStatus? Status { get; init; }
    public bool? InStock { get; init; }
    public int? MinQuantity { get; init; }
}
