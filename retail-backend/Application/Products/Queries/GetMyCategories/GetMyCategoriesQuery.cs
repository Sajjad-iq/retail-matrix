using Application.Products.DTOs;
using Domains.Shared.Base;
using MediatR;

namespace Application.Products.Queries.GetMyCategories;

public record GetMyCategoriesQuery : IRequest<PagedResult<CategoryDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
