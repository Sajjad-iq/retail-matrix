using MediatR;

namespace Application.Products.Commands.UpdateProduct;

public record UpdateProductCommand : IRequest<Unit>
{
    public Guid Id { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public Guid? CategoryId { get; init; }
}
