using Application.Products.DTOs;
using Domains.Shared.Base;
using MediatR;
using Domains.Products.Enums;

namespace Application.Products.Queries.GetMyProducts;

public record GetMyProductsQuery : IRequest<PagedResult<ProductListDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public List<Guid>? Ids { get; init; }
    public Guid? CategoryId { get; init; }
    public List<Guid>? CategoryIds { get; init; }
    public string? SearchTerm { get; init; }
    public ProductStatus? Status { get; init; }
    public bool? IsSelling { get; init; }
    public bool? IsRawMaterial { get; init; }
}
