using Application.Products.DTOs;
using Domains.Shared.Base;
using MediatR;
using Domains.Products.Enums;

namespace Application.Products.Queries.GetMyPackagings;

public record GetMyPackagingsQuery : IRequest<PagedResult<ProductPackagingListDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public Guid? ProductId { get; init; }
    public ProductStatus? Status { get; init; }
    public string? SearchTerm { get; init; }
    public string? Barcode { get; init; }
    public bool? IsDefault { get; init; }
}
