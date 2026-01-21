using Domains.Organizations.Enums;

namespace Application.Organizations.DTOs;

public record OrganizationDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Domain { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public OrganizationStatus Status { get; init; }
    public Guid CreatedBy { get; init; }
    public string? LogoUrl { get; init; }
    public DateTime InsertDate { get; init; }
}
