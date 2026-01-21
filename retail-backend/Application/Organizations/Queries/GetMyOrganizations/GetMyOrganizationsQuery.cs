using Application.Organizations.DTOs;
using MediatR;

namespace Application.Organizations.Queries.GetMyOrganizations;

public record GetMyOrganizationsQuery : IRequest<List<OrganizationListDto>>
{
}
