using Application.Common.Exceptions;
using Domains.Stocks.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Stocks.Commands.AdjustStockQuantity;

public class AdjustStockQuantityCommandHandler : IRequestHandler<AdjustStockQuantityCommand, bool>
{
    private readonly IStockRepository _stockRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AdjustStockQuantityCommandHandler(
        IStockRepository stockRepository,
        IHttpContextAccessor httpContextAccessor)
    {
        _stockRepository = stockRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<bool> Handle(AdjustStockQuantityCommand request, CancellationToken cancellationToken)
    {
        // 1. Get organization ID from claims
        var orgIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("OrganizationId")?.Value;
        if (string.IsNullOrEmpty(orgIdClaim) || !Guid.TryParse(orgIdClaim, out var organizationId))
        {
            throw new UnauthorizedException("معرف المؤسسة مطلوب");
        }

        // 2. Get stock with batches
        var stock = await _stockRepository.GetWithBatchesAsync(request.StockId, cancellationToken);
        if (stock == null)
        {
            throw new NotFoundException("المخزون غير موجود");
        }

        // 3. Verify organization ownership
        if (stock.OrganizationId != organizationId)
        {
            throw new UnauthorizedException("غير مصرح لك بالوصول إلى هذا المخزون");
        }

        // 4. Get the batch
        var batch = stock.GetBatch(request.BatchId);
        if (batch == null)
        {
            throw new NotFoundException("الدفعة غير موجودة");
        }

        // 5. Apply quantity change
        if (request.QuantityChange > 0)
        {
            batch.AddQuantity(request.QuantityChange);
        }
        else if (request.QuantityChange < 0)
        {
            batch.RemoveQuantity(Math.Abs(request.QuantityChange));
        }

        // 6. Persist changes
        await _stockRepository.UpdateAsync(stock, cancellationToken);
        await _stockRepository.SaveChangesAsync(cancellationToken);

        return true;
    }
}
