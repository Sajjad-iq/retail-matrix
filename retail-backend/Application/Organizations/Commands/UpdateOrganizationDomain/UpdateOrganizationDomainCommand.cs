using MediatR;

namespace Application.Organizations.Commands.UpdateOrganizationDomain;

public record UpdateOrganizationDomainCommand : IRequest<Unit>
{
    public Guid OrganizationId { get; init; }
    public string Domain { get; init; } = string.Empty;
}
