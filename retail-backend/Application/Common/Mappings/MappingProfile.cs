using Application.Auth.DTOs;
using Application.Organizations.DTOs;
using Application.Inventory.DTOs;
using AutoMapper;
using Domains.Users.Entities;
using Domains.Organizations.Entities;
using Domains.Inventory.Entities;
using InventoryEntity = Domains.Inventory.Entities.Inventory;

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

        // Inventory mappings
        CreateMap<InventoryOperation, InventoryOperationDto>();
        CreateMap<InventoryOperation, InventoryOperationListDto>();
        CreateMap<InventoryOperationItem, InventoryOperationItemDto>();
        CreateMap<InventoryEntity, InventoryDto>();
    }
}
