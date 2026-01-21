using MediatR;

namespace Application.Products.Commands.CreateCategory;

public record CreateCategoryCommand : IRequest<Guid>
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public Guid? ParentCategoryId { get; init; }
}
