using Application.Common.Exceptions;
using Application.Common.Services;
using Application.POS.DTOs;
using Domains.Sales.Repositories;
using Domains.Stocks.Repositories;
using MediatR;

namespace Application.POS.Commands.CompleteSale;

public class CompleteSaleCommandHandler : IRequestHandler<CompleteSaleCommand, CompletedSaleDto>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IStockRepository _stockRepository;
    private readonly IOrganizationContext _organizationContext;

    public CompleteSaleCommandHandler(
        ISaleRepository saleRepository,
        IStockRepository stockRepository,
        IOrganizationContext organizationContext)
    {
        _saleRepository = saleRepository;
        _stockRepository = stockRepository;
        _organizationContext = organizationContext;
    }

    public async Task<CompletedSaleDto> Handle(CompleteSaleCommand request, CancellationToken cancellationToken)
    {
        var organizationId = _organizationContext.OrganizationId;

        var sale = await _saleRepository.GetByIdWithTrackingAsync(request.SaleId, cancellationToken);
        if (sale == null)
        {
            throw new NotFoundException("البيع غير موجود");
        }

        if (sale.OrganizationId != organizationId)
        {
            throw new UnauthorizedException("غير مصرح بالوصول إلى هذا البيع");
        }

        // CRITICAL: Check if sale is already completed to prevent duplicate processing
        if (sale.Status == Domains.Sales.Enums.SaleStatus.Completed)
        {
            throw new ValidationException("البيع مكتمل بالفعل");
        }

        // Deduct stock for each item
        foreach (var item in sale.Items)
        {
            var stock = await _stockRepository.GetByPackagingAsync(
                item.ProductPackagingId,
                organizationId,
                request.InventoryId,
                cancellationToken);

            if (stock == null)
            {
                throw new ValidationException($"المخزون غير موجود للمنتج: {item.ProductName}");
            }

            // Deduct from batches (no reservation release needed)
            var remainingQuantity = item.Quantity;
            var availableBatches = stock.GetAvailableBatches().ToList();

            foreach (var batch in availableBatches)
            {
                if (remainingQuantity <= 0) break;

                var quantityToDeduct = Math.Min(remainingQuantity, batch.AvailableQuantity);
                batch.RemoveQuantity(quantityToDeduct);
                remainingQuantity -= quantityToDeduct;
            }

            if (remainingQuantity > 0)
            {
                throw new ValidationException($"الكمية المتاحة غير كافية للمنتج: {item.ProductName}");
            }
        }

        // Record payment amount
        var paymentAmount = Domains.Shared.ValueObjects.Price.Create(
            request.AmountPaid,
            sale.GrandTotal.Currency
        );
        sale.RecordPayment(paymentAmount);

        // Complete the sale
        sale.CompleteSale();

        // Save all changes once (both sale and stock changes will be persisted together)
        await _saleRepository.SaveChangesAsync(cancellationToken);

        // Calculate change
        var change = Domains.Shared.ValueObjects.Price.Create(
            request.AmountPaid - sale.GrandTotal.Amount,
            sale.GrandTotal.Currency
        );

        // Map items with remaining stock (after deduction)
        var itemDtos = new List<PosCartItemDto>();
        foreach (var item in sale.Items)
        {
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

        // Return completed sale DTO
        return new CompletedSaleDto
        {
            SaleId = sale.Id,
            SaleNumber = sale.SaleNumber,
            SaleDate = sale.SaleDate,
            CompletedAt = DateTime.UtcNow,
            Items = itemDtos,
            TotalDiscount = sale.TotalDiscount,
            GrandTotal = sale.GrandTotal,
            AmountPaid = sale.AmountPaid,
            Change = change
        };
    }
}
