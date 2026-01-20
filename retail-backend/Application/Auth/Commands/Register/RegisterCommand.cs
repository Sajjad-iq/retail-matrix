using MediatR;

namespace Application.Auth.Commands.Register;

public record RegisterCommand : IRequest<Guid>
{
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public string? Address { get; init; }
}
