using Application.Common.Exceptions;
using Domains.Stocks.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Stocks.Commands.AddStockBatch;

public class AddStockBatchCommandHandler : IRequestHandler<AddStockBatchCommand, Guid>
{
    private readonly IStockRepository _stockRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AddStockBatchCommandHandler(
        IStockRepository stockRepository,
        IHttpContextAccessor httpContextAccessor)
    {
        _stockRepository = stockRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Guid> Handle(AddStockBatchCommand request, CancellationToken cancellationToken)
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

        // 4. Check if batch number already exists
        var existingBatch = stock.GetBatchByNumber(request.BatchNumber);
        if (existingBatch != null)
        {
            throw new ValidationException("رقم الدفعة موجود بالفعل في هذا المخزون");
        }

        // 5. Add batch using domain method
        var batch = stock.AddBatch(
            batchNumber: request.BatchNumber,
            quantity: request.Quantity,
            expiryDate: request.ExpiryDate,
            condition: request.Condition,
            costPrice: request.CostPrice
        );

        // 6. Persist changes
        await _stockRepository.UpdateAsync(stock, cancellationToken);
        await _stockRepository.SaveChangesAsync(cancellationToken);

        return batch.Id;
    }
}
