using Application.Common.Exceptions;
using Domains.Organizations.Repositories;
using MediatR;

namespace Application.Organizations.Commands.UpdateOrganization;

public class UpdateOrganizationCommandHandler : IRequestHandler<UpdateOrganizationCommand, Unit>
{
    private readonly IOrganizationRepository _organizationRepository;

    public UpdateOrganizationCommandHandler(IOrganizationRepository organizationRepository)
    {
        _organizationRepository = organizationRepository;
    }

    public async Task<Unit> Handle(UpdateOrganizationCommand request, CancellationToken cancellationToken)
    {
        // 1. Retrieve organization
        var organization = await _organizationRepository.GetByIdAsync(request.OrganizationId, cancellationToken);
        if (organization == null)
        {
            throw new NotFoundException("المؤسسة غير موجودة");
        }

        // 2. Update profile using domain method
        organization.UpdateProfile(
            name: request.Name,
            description: request.Description,
            address: request.Address,
            logoUrl: request.LogoUrl
        );

        // 3. Save changes
        await _organizationRepository.UpdateAsync(organization, cancellationToken);
        await _organizationRepository.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
