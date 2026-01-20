using System.Security.Claims;
using Domains.Users.Entities;

namespace Application.Common.Services;

public interface IJwtTokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    ClaimsPrincipal? ValidateToken(string token);
    DateTime GetTokenExpiration();
}
