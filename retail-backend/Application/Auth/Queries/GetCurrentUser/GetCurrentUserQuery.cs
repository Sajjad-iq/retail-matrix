using Application.Auth.DTOs;
using MediatR;

namespace Application.Auth.Queries.GetCurrentUser;

public record GetCurrentUserQuery(Guid UserId) : IRequest<UserDto?>;
