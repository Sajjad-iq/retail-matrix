using Application.Common.Exceptions;
using Application.Common.Services;
using Application.POS.DTOs;
using Domains.Sales.Repositories;
using MediatR;

namespace Application.POS.Queries.GetSale;

public class GetSaleQueryHandler : IRequestHandler<GetSaleQuery, SaleDto>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IOrganizationContext _organizationContext;

    public GetSaleQueryHandler(
        ISaleRepository saleRepository,
        IOrganizationContext organizationContext)
    {
        _saleRepository = saleRepository;
        _organizationContext = organizationContext;
    }

    public async Task<SaleDto> Handle(GetSaleQuery request, CancellationToken cancellationToken)
    {
        var organizationId = _organizationContext.OrganizationId;

        // Get the sale
        var sale = await _saleRepository.GetByIdAsync(request.SaleId, cancellationToken);
        if (sale == null)
        {
            throw new NotFoundException("البيع غير موجود");
        }

        if (sale.OrganizationId != organizationId)
        {
            throw new UnauthorizedException("غير مصرح بالوصول إلى هذا البيع");
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
