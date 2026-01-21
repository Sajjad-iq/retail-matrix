using Application.Products.DTOs;
using MediatR;

namespace Application.Products.Queries.GetCategoryById;

public record GetCategoryByIdQuery : IRequest<CategoryDto>
{
    public Guid CategoryId { get; init; }
}
