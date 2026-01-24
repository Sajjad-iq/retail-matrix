using System.Security.Claims;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Infrastructure.Filters;

/// <summary>
/// Authorization filter that validates the C-Organization header against user's allowed organizations.
/// - For Business Owners: Checks if the organization ID is in the OwnedOrganizations claim
/// - For Employees: Checks if the organization ID matches the MemberOfOrganization claim
/// </summary>
public class OrganizationAuthorizationFilter : IAuthorizationFilter
{
    private const string OrganizationHeaderName = "C-Organization";

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;

        // Check if user is authenticated
        if (!user.Identity?.IsAuthenticated ?? true)
        {
            context.Result = new UnauthorizedObjectResult(
                ApiErrorResponse.Create("غير مصرح - يجب تسجيل الدخول")
            );
            return;
        }

        // Get organization ID from header
        if (!context.HttpContext.Request.Headers.TryGetValue(OrganizationHeaderName, out var organizationHeader))
        {
            context.Result = new UnauthorizedObjectResult(
                ApiErrorResponse.Create($"رأس {OrganizationHeaderName} مطلوب")
            );
            return;
        }

        var organizationIdString = organizationHeader.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(organizationIdString) || !Guid.TryParse(organizationIdString, out var requestedOrganizationId))
        {
            context.Result = new UnauthorizedObjectResult(
                ApiErrorResponse.Create("معرف المؤسسة غير صالح")
            );
            return;
        }

        // Get user claims
        var ownedOrganizationsClaim = user.FindFirst("OwnedOrganizations")?.Value;
        var memberOfOrganizationClaim = user.FindFirst("MemberOfOrganization")?.Value;

        bool isOwner = false;
        bool isEmployee = false;

        // Check if user owns this organization (Business Owner)
        if (!string.IsNullOrEmpty(ownedOrganizationsClaim))
        {
            var ownedOrgIds = ownedOrganizationsClaim
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(id => Guid.TryParse(id.Trim(), out var guid) ? guid : Guid.Empty)
                .Where(g => g != Guid.Empty)
                .ToList();

            if (ownedOrgIds.Contains(requestedOrganizationId))
            {
                isOwner = true;
            }
        }

        // Check if user is an employee of this organization
        if (!isOwner && !string.IsNullOrEmpty(memberOfOrganizationClaim))
        {
            if (Guid.TryParse(memberOfOrganizationClaim, out var memberOrgId) && memberOrgId == requestedOrganizationId)
            {
                isEmployee = true;
            }
        }

        // If user has no access to this organization
        if (!isOwner && !isEmployee)
        {
            context.Result = new UnauthorizedObjectResult(
                ApiErrorResponse.Create("غير مصرح - لا تملك صلاحية الوصول إلى هذه المؤسسة")
            );
            return;
        }

        // Set context items for use by handlers
        context.HttpContext.Items["CurrentOrganizationId"] = requestedOrganizationId;
        context.HttpContext.Items["IsOrganizationOwner"] = isOwner;
        context.HttpContext.Items["IsOrganizationEmployee"] = isEmployee;
    }
}
