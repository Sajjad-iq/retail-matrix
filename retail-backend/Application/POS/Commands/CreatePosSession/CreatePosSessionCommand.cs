using MediatR;

namespace Application.POS.Commands.CreatePosSession;

/// <summary>
/// Command to create a new POS session (Draft Sale)
/// </summary>
public record CreatePosSessionCommand : IRequest<Guid>
{
    public Guid? InventoryId { get; init; }
}
