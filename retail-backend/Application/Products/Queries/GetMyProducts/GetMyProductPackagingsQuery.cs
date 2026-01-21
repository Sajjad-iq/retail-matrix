using Application.Products.DTOs;
using Domains.Shared.Base;
using MediatR;

namespace Application.Products.Queries.GetMyProductPackagings;

public record GetMyProductPackagingsQuery : IRequest<PagedResult<ProductPackagingListDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
