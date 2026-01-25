using MediatR;

namespace Application.Products.Commands.DeleteProduct;

public record DeleteProductCommand : IRequest<Unit>
{
    public Guid Id { get; init; }
}
