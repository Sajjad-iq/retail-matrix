using MediatR;

namespace Application.Products.Commands.DeleteProductPackaging;

public record DeleteProductPackagingCommand : IRequest<Unit>
{
    public Guid Id { get; init; }
}
