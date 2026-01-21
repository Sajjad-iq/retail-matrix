using Application.Common.Exceptions;
using Domains.Sales.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.POS.Commands.RecordPosPayment;

public class RecordPosPaymentCommandHandler : IRequestHandler<RecordPosPaymentCommand, bool>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RecordPosPaymentCommandHandler(
        ISaleRepository saleRepository,
        IHttpContextAccessor httpContextAccessor)
    {
        _saleRepository = saleRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<bool> Handle(RecordPosPaymentCommand request, CancellationToken cancellationToken)
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

        // Record payment using domain method with Price value object
        sale.RecordPayment(request.Amount);

        await _saleRepository.SaveChangesAsync(cancellationToken);
        return true;
    }
}
