using Application.Common.Exceptions;
using Domains.Organizations.Enums;
using Domains.Organizations.Repositories;
using MediatR;

namespace Application.Organizations.Commands.ChangeOrganizationStatus;

public class ChangeOrganizationStatusCommandHandler : IRequestHandler<ChangeOrganizationStatusCommand, Unit>
{
    private readonly IOrganizationRepository _organizationRepository;

    public ChangeOrganizationStatusCommandHandler(IOrganizationRepository organizationRepository)
    {
        _organizationRepository = organizationRepository;
    }

    public async Task<Unit> Handle(ChangeOrganizationStatusCommand request, CancellationToken cancellationToken)
    {
        // 1. Retrieve organization
        var organization = await _organizationRepository.GetByIdAsync(request.OrganizationId, cancellationToken);
        if (organization == null)
        {
            throw new NotFoundException("المؤسسة غير موجودة");
        }

        // 2. Call appropriate domain method based on status
        switch (request.Status)
        {
            case OrganizationStatus.Active:
                if (organization.IsPending())
                {
                    organization.ApprovePending();
                }
                else
                {
                    organization.Activate();
                }
                break;

            case OrganizationStatus.Suspended:
                organization.Suspend();
                break;

            case OrganizationStatus.Closed:
                organization.Close();
                break;

            case OrganizationStatus.Pending:
                throw new ValidationException("لا يمكن تغيير الحالة إلى قيد الانتظار");

            default:
                throw new ValidationException("حالة المؤسسة غير صحيحة");
        }

        // 3. Save changes
        await _organizationRepository.UpdateAsync(organization, cancellationToken);
        await _organizationRepository.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
