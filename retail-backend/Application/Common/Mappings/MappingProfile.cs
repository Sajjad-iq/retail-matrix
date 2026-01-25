using Application.Auth.DTOs;
using Application.Organizations.DTOs;
using Application.Inventory.DTOs;
using Application.Products.DTOs;
using Application.Stocks.DTOs;
using AutoMapper;
using Domains.Users.Entities;
using Domains.Organizations.Entities;
using Domains.Inventory.Entities;
using Domains.Products.Entities;
using Domains.Stocks.Entities;
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

        // Product mappings
        CreateMap<Product, ProductWithPackagingsDto>()
            .ForMember(d => d.CategoryName, opt => opt.MapFrom(s => s.Category != null ? s.Category.Name : null));

        // ProductPackaging mappings
        CreateMap<ProductPackaging, ProductPackagingDto>();
        CreateMap<ProductPackaging, ProductPackagingListDto>();

        // Category mappings
        CreateMap<Category, CategoryDto>();

        // Stock mappings
        CreateMap<Stock, StockDto>()
            .ForMember(d => d.Batches, opt => opt.MapFrom(s => s.Batches));
        CreateMap<Stock, StockListDto>();

        // StockBatch mappings
        CreateMap<StockBatch, StockBatchDto>();
    }
}
