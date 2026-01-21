using Application.Common.Exceptions;
using Application.Organizations.DTOs;
using AutoMapper;
using Domains.Organizations.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Application.Organizations.Queries.GetMyOrganizations;

public class GetMyOrganizationsQueryHandler : IRequestHandler<GetMyOrganizationsQuery, List<OrganizationListDto>>
{
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetMyOrganizationsQueryHandler(
        IOrganizationRepository organizationRepository,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor)
    {
        _organizationRepository = organizationRepository;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<List<OrganizationListDto>> Handle(GetMyOrganizationsQuery request, CancellationToken cancellationToken)
    {
        // Get current user ID from claims
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedException("المستخدم غير مصرح له");
        }

        var organizations = await _organizationRepository.GetByCreatorAsync(userId, cancellationToken);
        return _mapper.Map<List<OrganizationListDto>>(organizations);
    }
}
