using Application.Common.Exceptions;
using Application.POS.DTOs;
using Domains.Sales.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.POS.Queries.GetPosCart;

public class GetPosCartQueryHandler : IRequestHandler<GetPosCartQuery, PosCartDto>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetPosCartQueryHandler(
        ISaleRepository saleRepository,
        IHttpContextAccessor httpContextAccessor)
    {
        _saleRepository = saleRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<PosCartDto> Handle(GetPosCartQuery request, CancellationToken cancellationToken)
    {
        var orgIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("OrganizationId")?.Value;
        if (string.IsNullOrEmpty(orgIdClaim) || !Guid.TryParse(orgIdClaim, out var organizationId))
        {
            throw new UnauthorizedException("معرف المؤسسة مطلوب");
        }

        var sale = await _saleRepository.GetByIdAsync(request.SaleId, cancellationToken);
        if (sale == null)
        {
            throw new NotFoundException("جلسة البيع غير موجودة");
        }

        if (sale.OrganizationId != organizationId)
        {
            throw new UnauthorizedException("غير مصرح بالوصول إلى هذه الجلسة");
        }

        return new PosCartDto
        {
            SaleId = sale.Id,
            SaleNumber = sale.SaleNumber,
            SaleDate = sale.SaleDate,
            Status = sale.Status,
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
