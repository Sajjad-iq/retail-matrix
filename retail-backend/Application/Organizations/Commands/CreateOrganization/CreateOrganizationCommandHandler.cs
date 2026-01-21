using Application.Common.Exceptions;
using Domains.Organizations.Entities;
using Domains.Organizations.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Application.Organizations.Commands.CreateOrganization;

public class CreateOrganizationCommandHandler : IRequestHandler<CreateOrganizationCommand, Guid>
{
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CreateOrganizationCommandHandler(
        IOrganizationRepository organizationRepository,
        IHttpContextAccessor httpContextAccessor)
    {
        _organizationRepository = organizationRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Guid> Handle(CreateOrganizationCommand request, CancellationToken cancellationToken)
    {
        // 1. Check if domain already exists
        if (await _organizationRepository.ExistsByDomainAsync(request.Domain, cancellationToken))
        {
            throw new ValidationException("نطاق المؤسسة مستخدم بالفعل");
        }

        // 2. Get current user ID from claims
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedException("المستخدم غير مصرح له");
        }

        // 3. Create organization using factory method
        var organization = Organization.Create(
            name: request.Name,
            domain: request.Domain,
            phone: request.Phone,
            createdBy: userId,
            description: request.Description,
            address: request.Address,
            logoUrl: request.LogoUrl
        );

        // 4. Persist organization
        await _organizationRepository.AddAsync(organization, cancellationToken);
        await _organizationRepository.SaveChangesAsync(cancellationToken);

        // 5. Return organization ID
        return organization.Id;
    }
}
