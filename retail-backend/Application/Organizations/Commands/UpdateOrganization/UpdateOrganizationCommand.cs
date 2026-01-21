using MediatR;

namespace Application.Organizations.Commands.UpdateOrganization;

public record UpdateOrganizationCommand : IRequest<Unit>
{
    public Guid OrganizationId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? Address { get; init; }
    public string? LogoUrl { get; init; }
}
