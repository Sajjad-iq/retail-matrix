using Application.Auth.DTOs;
using Application.Organizations.DTOs;
using AutoMapper;
using Domains.Users.Entities;
using Domains.Organizations.Entities;

namespace Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User mappings
        CreateMap<User, UserDto>()
            .ForMember(d => d.UserRoles, opt => opt.MapFrom(s => s.UserRoles));

        // Organization mappings
        CreateMap<Organization, OrganizationDto>();
        CreateMap<Organization, OrganizationListDto>();
    }
}
