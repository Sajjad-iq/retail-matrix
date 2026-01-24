using System.Security.Claims;
using Application.Common.Services;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services;

/// <summary>
/// Implementation of IOrganizationContext that extracts organization info from HTTP context.
/// The organization ID is set by the OrganizationAuthorizationFilter after validation.
/// </summary>
public class OrganizationContext : IOrganizationContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public OrganizationContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid OrganizationId
    {
        get
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
                throw new InvalidOperationException("HttpContext is not available");

            // Get from Items set by OrganizationAuthorizationFilter
            if (httpContext.Items.TryGetValue("CurrentOrganizationId", out var orgId) && orgId is Guid organizationId)
            {
                return organizationId;
            }

            throw new InvalidOperationException("Organization context is not available. Ensure the endpoint uses [OrganizationAuthorize] attribute.");
        }
    }

    public Guid UserId
    {
        get
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
                throw new InvalidOperationException("HttpContext is not available");

            var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                throw new InvalidOperationException("User ID not found in claims");
            }

            return userId;
        }
    }

    public bool IsOrganizationOwner
    {
        get
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
                return false;

            if (httpContext.Items.TryGetValue("IsOrganizationOwner", out var isOwner) && isOwner is bool ownerFlag)
            {
                return ownerFlag;
            }

            return false;
        }
    }

    public bool IsOrganizationEmployee
    {
        get
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
                return false;

            if (httpContext.Items.TryGetValue("IsOrganizationEmployee", out var isEmployee) && isEmployee is bool employeeFlag)
            {
                return employeeFlag;
            }

            return false;
        }
    }
}
