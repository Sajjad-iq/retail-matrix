using MediatR;

namespace Application.POS.Commands.CancelPosSession;

/// <summary>
/// Command to cancel a POS session
/// </summary>
public record CancelPosSessionCommand : IRequest<bool>
{
    public Guid SaleId { get; init; }
}
