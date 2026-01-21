using Application.Common.Exceptions;
using Application.POS.DTOs;
using Domains.Sales.Repositories;
using Domains.Stocks.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.POS.Commands.CompletePosSession;

public class CompletePosSessionCommandHandler : IRequestHandler<CompletePosSessionCommand, CompletedSaleDto>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IStockRepository _stockRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CompletePosSessionCommandHandler(
        ISaleRepository saleRepository,
        IStockRepository stockRepository,
        IHttpContextAccessor httpContextAccessor)
    {
        _saleRepository = saleRepository;
        _stockRepository = stockRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<CompletedSaleDto> Handle(CompletePosSessionCommand request, CancellationToken cancellationToken)
    {
        var orgIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("OrganizationId")?.Value;
        if (string.IsNullOrEmpty(orgIdClaim) || !Guid.TryParse(orgIdClaim, out var organizationId))
        {
            throw new UnauthorizedException("معرف المؤسسة مطلوب");
        }

        var sale = await _saleRepository.GetByIdWithTrackingAsync(request.SaleId, cancellationToken);
        if (sale == null)
        {
            throw new NotFoundException("جلسة البيع غير موجودة");
        }

        if (sale.OrganizationId != organizationId)
        {
            throw new UnauthorizedException("غير مصرح بالوصول إلى هذه الجلسة");
        }

        // Deduct stock for each item using FEFO
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

            // Deduct from batches using FEFO (First Expired, First Out)
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

            await _stockRepository.UpdateAsync(stock, cancellationToken);
        }

        // Complete the sale using domain method
        sale.CompleteSale();

        await _saleRepository.SaveChangesAsync(cancellationToken);
        await _stockRepository.SaveChangesAsync(cancellationToken);

        // Return completed sale DTO
        return new CompletedSaleDto
        {
            SaleId = sale.Id,
            SaleNumber = sale.SaleNumber,
            SaleDate = sale.SaleDate,
            CompletedAt = DateTime.UtcNow,
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
            AmountPaid = sale.AmountPaid
        };
    }
}
