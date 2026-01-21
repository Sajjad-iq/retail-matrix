using MediatR;

namespace Application.Organizations.Commands.CreateOrganization;

public record CreateOrganizationCommand : IRequest<Guid>
{
    public string Name { get; init; } = string.Empty;
    public string Domain { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? Address { get; init; }
    public string? LogoUrl { get; init; }
}
