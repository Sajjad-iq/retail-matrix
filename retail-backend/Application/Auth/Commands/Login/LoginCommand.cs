using Application.Auth.DTOs;
using MediatR;

namespace Application.Auth.Commands.Login;

public record LoginCommand : IRequest<TokenDto>
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}
