using Domains.Organizations.Enums;
using MediatR;

namespace Application.Organizations.Commands.ChangeOrganizationStatus;

public record ChangeOrganizationStatusCommand : IRequest<Unit>
{
    public Guid OrganizationId { get; init; }
    public OrganizationStatus Status { get; init; }
}
