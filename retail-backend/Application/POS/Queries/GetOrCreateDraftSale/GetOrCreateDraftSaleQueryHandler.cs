using Application.Common.Services;
using Application.POS.DTOs;
using Domains.Sales.Entities;
using Domains.Sales.Enums;
using Domains.Sales.Repositories;
using Domains.Shared.ValueObjects;
using MediatR;

namespace Application.POS.Queries.GetOrCreateDraftSale;

public class GetOrCreateDraftSaleQueryHandler : IRequestHandler<GetOrCreateDraftSaleQuery, SaleDto>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IOrganizationContext _organizationContext;

    public GetOrCreateDraftSaleQueryHandler(
        ISaleRepository saleRepository,
        IOrganizationContext organizationContext)
    {
        _saleRepository = saleRepository;
        _organizationContext = organizationContext;
    }

    public async Task<SaleDto> Handle(GetOrCreateDraftSaleQuery request, CancellationToken cancellationToken)
    {
        var organizationId = _organizationContext.OrganizationId;
        var userId = _organizationContext.UserId;

        // Try to find existing draft sale for this inventory and user
        var existingSale = await _saleRepository.GetActiveDraftSaleAsync(
            request.InventoryId, 
            userId, 
            cancellationToken);

        Sale sale;
        
        if (existingSale != null)
        {
            sale = existingSale;
        }
        else
        {
            // Create new draft sale
            sale = Sale.Create(
                organizationId,
                userId);

            await _saleRepository.AddAsync(sale, cancellationToken);
            await _saleRepository.SaveChangesAsync(cancellationToken);
        }

        // Map to DTO
        return new SaleDto
        {
            SaleId = sale.Id,
            SaleNumber = sale.SaleNumber,
            SaleDate = sale.SaleDate,
            Status = sale.Status.ToString(),
            Items = sale.Items.Select(i => new PosCartItemDto
            {
                ItemId = i.Id,
                ProductPackagingId = i.ProductPackagingId,
                ProductName = i.ProductName,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                Discount = i.Discount,
                LineTotal = i.LineTotal
            }).ToList(),
            TotalDiscount = sale.TotalDiscount,
            GrandTotal = sale.GrandTotal,
            AmountPaid = sale.AmountPaid,
            Notes = sale.Notes
        };
    }
}
