using Application.Auth.DTOs;
using AutoMapper;
using Domains.Users.Entities;

namespace Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User mappings
        CreateMap<User, UserDto>()
            .ForMember(d => d.AccountType, opt => opt.MapFrom(s => s.AccountType.ToString()))
            .ForMember(d => d.Roles, opt => opt.MapFrom(s => s.UserRoles.Select(r => r.ToString()).ToList()));
    }
}
