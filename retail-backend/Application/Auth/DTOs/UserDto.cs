using Domains.Users.Enums;

namespace Application.Auth.DTOs;

public record UserDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public AccountType AccountType { get; init; }
    public HashSet<Roles> UserRoles { get; init; } = new();
    public bool IsActive { get; init; }
    public bool EmailVerified { get; init; }
    public bool PhoneVerified { get; init; }
}
