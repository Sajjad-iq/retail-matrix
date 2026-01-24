using System.Security.Claims;
using Domains.Users.Entities;

namespace Application.Common.Services;

public interface IJwtTokenService
{
    string GenerateAccessToken(User user, IEnumerable<Guid>? ownedOrganizationIds = null);
    string GenerateRefreshToken();
    ClaimsPrincipal? ValidateToken(string token);
    DateTime GetTokenExpiration();
}
