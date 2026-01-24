using Microsoft.AspNetCore.Mvc;

namespace Infrastructure.Filters;

/// <summary>
/// Attribute to mark endpoints that require organization authorization.
/// The C-Organization header must be provided and the user must have access to the organization.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class OrganizationAuthorizeAttribute : TypeFilterAttribute
{
    public OrganizationAuthorizeAttribute() : base(typeof(OrganizationAuthorizationFilter))
    {
    }
}
