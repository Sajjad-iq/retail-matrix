using Domains.Organizations.Enums;

namespace Application.Organizations.DTOs;

public record OrganizationListDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Domain { get; init; } = string.Empty;
    public OrganizationStatus Status { get; init; }
    public string Phone { get; init; } = string.Empty;
    public string? LogoUrl { get; init; }
}
