using Application.Common.Exceptions;
using Domains.Organizations.Repositories;
using MediatR;

namespace Application.Organizations.Commands.UpdateOrganizationDomain;

public class UpdateOrganizationDomainCommandHandler : IRequestHandler<UpdateOrganizationDomainCommand, Unit>
{
    private readonly IOrganizationRepository _organizationRepository;

    public UpdateOrganizationDomainCommandHandler(IOrganizationRepository organizationRepository)
    {
        _organizationRepository = organizationRepository;
    }

    public async Task<Unit> Handle(UpdateOrganizationDomainCommand request, CancellationToken cancellationToken)
    {
        // 1. Check if domain already exists (excluding current organization)
        var existingOrg = await _organizationRepository.GetByDomainAsync(request.Domain, cancellationToken);
        if (existingOrg != null && existingOrg.Id != request.OrganizationId)
        {
            throw new ValidationException("نطاق المؤسسة مستخدم بالفعل");
        }

        // 2. Retrieve organization
        var organization = await _organizationRepository.GetByIdAsync(request.OrganizationId, cancellationToken);
        if (organization == null)
        {
            throw new NotFoundException("المؤسسة غير موجودة");
        }

        // 3. Update domain using domain method
        organization.UpdateDomain(request.Domain);

        // 4. Save changes
        await _organizationRepository.UpdateAsync(organization, cancellationToken);
        await _organizationRepository.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
