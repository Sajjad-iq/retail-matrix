using Application.Organizations.DTOs;
using MediatR;

namespace Application.Organizations.Queries.GetOrganizationById;

public record GetOrganizationByIdQuery : IRequest<OrganizationDto>
{
    public Guid OrganizationId { get; init; }
}
