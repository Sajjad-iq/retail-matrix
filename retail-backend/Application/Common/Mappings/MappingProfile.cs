using Application.Auth.DTOs;
using AutoMapper;
using Domains.Users.Entities;

namespace Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User mappings
        CreateMap<User, UserDto>();
    }
}
