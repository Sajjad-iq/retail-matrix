using Application.Products.DTOs;
using Domains.Shared.Base;
using MediatR;

namespace Application.Products.Queries.GetMyProducts;

public record GetMyProductsQuery : IRequest<PagedResult<ProductWithPackagingsDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
