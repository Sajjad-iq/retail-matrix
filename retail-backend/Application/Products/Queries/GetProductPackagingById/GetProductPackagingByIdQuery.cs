using Application.Products.DTOs;
using MediatR;

namespace Application.Products.Queries.GetProductPackagingById;

public record GetProductPackagingByIdQuery : IRequest<ProductPackagingDto>
{
    public Guid PackagingId { get; init; }
}
