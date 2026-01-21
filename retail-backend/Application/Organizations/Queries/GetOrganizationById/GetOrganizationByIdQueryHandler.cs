using Application.Common.Exceptions;
using Application.Organizations.DTOs;
using AutoMapper;
using Domains.Organizations.Repositories;
using MediatR;

namespace Application.Organizations.Queries.GetOrganizationById;

public class GetOrganizationByIdQueryHandler : IRequestHandler<GetOrganizationByIdQuery, OrganizationDto>
{
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IMapper _mapper;

    public GetOrganizationByIdQueryHandler(
        IOrganizationRepository organizationRepository,
        IMapper mapper)
    {
        _organizationRepository = organizationRepository;
        _mapper = mapper;
    }

    public async Task<OrganizationDto> Handle(GetOrganizationByIdQuery request, CancellationToken cancellationToken)
    {
        var organization = await _organizationRepository.GetByIdAsync(request.OrganizationId, cancellationToken);

        if (organization == null)
        {
            throw new NotFoundException("المؤسسة غير موجودة");
        }

        return _mapper.Map<OrganizationDto>(organization);
    }
}
