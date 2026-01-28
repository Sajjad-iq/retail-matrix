using Application.Auth.DTOs;
using Application.Organizations.DTOs;
using Application.Inventory.DTOs;
using Application.Products.DTOs;
using Application.Stocks.DTOs;
using Application.Currencies.DTOs;
using AutoMapper;
using Domains.Users.Entities;
using Domains.Organizations.Entities;
using Domains.Inventory.Entities;
using Domains.Products.Entities;
using Domains.Stocks.Entities;
using Domains.Common.Currency.Entities;
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
        CreateMap<InventoryOperation, InventoryOperationListDto>()
            .ForMember(d => d.UserName, opt => opt.MapFrom(s => s.User != null ? s.User.Name : "Unknown"))
            .ForMember(d => d.UserAvatar, opt => opt.MapFrom(s => s.User != null ? s.User.Avatar : null))
            .ForMember(d => d.SourceInventoryName, opt => opt.MapFrom(s => s.SourceInventory != null ? s.SourceInventory.Name : null))
            .ForMember(d => d.DestinationInventoryName, opt => opt.MapFrom(s => s.DestinationInventory != null ? s.DestinationInventory.Name : null));
        CreateMap<InventoryOperationItem, InventoryOperationItemDto>();
        CreateMap<InventoryEntity, InventoryDto>();

        // Product mappings
        CreateMap<Product, ProductListDto>()
            .ForMember(d => d.CategoryName, opt => opt.MapFrom(s => s.Category != null ? s.Category.Name : null));

        // ProductPackaging mappings
        CreateMap<ProductPackaging, ProductPackagingDto>();
        CreateMap<ProductPackaging, ProductPackagingListDto>();

        // Category mappings
        CreateMap<Category, CategoryDto>();

        // Stock mappings
        CreateMap<Stock, StockDto>()
            .ForMember(d => d.Batches, opt => opt.MapFrom(s => s.Batches));
        CreateMap<Stock, StockListDto>()
             .ForMember(d => d.ProductName, opt => opt.MapFrom(s => s.ProductPackaging != null && s.ProductPackaging.Product != null ? s.ProductPackaging.Product.Name : null))
             .ForMember(d => d.PackagingName, opt => opt.MapFrom(s => s.ProductPackaging != null ? s.ProductPackaging.Name : null))
             .ForMember(d => d.InventoryName, opt => opt.MapFrom(s => s.Inventory != null ? s.Inventory.Name : null));

        // StockBatch mappings
        CreateMap<StockBatch, StockBatchDto>();

        // Currency mappings
        CreateMap<Currency, CurrencyDto>();
    }
}
