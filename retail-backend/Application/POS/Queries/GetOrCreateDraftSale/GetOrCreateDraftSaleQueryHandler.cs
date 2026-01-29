using Application.Common.Services;
using Application.POS.DTOs;
using Domains.Sales.Entities;
using Domains.Sales.Enums;
using Domains.Sales.Repositories;
using Domains.Shared.ValueObjects;
using Domains.Stocks.Repositories;
using MediatR;

namespace Application.POS.Queries.GetOrCreateDraftSale;

public class GetOrCreateDraftSaleQueryHandler : IRequestHandler<GetOrCreateDraftSaleQuery, SaleDto>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IStockRepository _stockRepository;
    private readonly IOrganizationContext _organizationContext;

    public GetOrCreateDraftSaleQueryHandler(
        ISaleRepository saleRepository,
        IStockRepository stockRepository,
        IOrganizationContext organizationContext)
    {
        _saleRepository = saleRepository;
        _stockRepository = stockRepository;
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
            // IMPORTANT: Always use the existing draft sale to prevent duplicates
            sale = existingSale;
        }
        else
        {
            // Create new draft sale only if none exists
            sale = Sale.Create(
                organizationId,
                userId);

            // Add notes to track which inventory this sale is for
            sale.AddNotes($"InventoryId:{request.InventoryId}");

            await _saleRepository.AddAsync(sale, cancellationToken);
            await _saleRepository.SaveChangesAsync(cancellationToken);
        }

        // Map to DTO with current stock information
        var itemDtos = new List<PosCartItemDto>();
        foreach (var item in sale.Items)
        {
            // Get current available stock
            var stock = await _stockRepository.GetByPackagingAsync(
                item.ProductPackagingId,
                organizationId,
                request.InventoryId,
                cancellationToken);

            itemDtos.Add(new PosCartItemDto
            {
                ItemId = item.Id,
                ProductPackagingId = item.ProductPackagingId,
                ProductName = item.ProductName,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                Discount = item.Discount,
                LineTotal = item.LineTotal,
                AvailableStock = stock?.TotalAvailableQuantity ?? 0
            });
        }

        return new SaleDto
        {
            SaleId = sale.Id,
            SaleNumber = sale.SaleNumber,
            SaleDate = sale.SaleDate,
            Status = sale.Status.ToString(),
            Items = itemDtos,
            TotalDiscount = sale.TotalDiscount,
            GrandTotal = sale.GrandTotal,
            AmountPaid = sale.AmountPaid,
            Notes = sale.Notes
        };
    }
}
