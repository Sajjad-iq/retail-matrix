using Application.Common.Exceptions;
using Domains.Stocks.Entities;
using Domains.Stocks.Enums;
using Domains.Stocks.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Stocks.Commands.CreateStock;

public class CreateStockCommandHandler : IRequestHandler<CreateStockCommand, Guid>
{
    private readonly IStockRepository _stockRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CreateStockCommandHandler(
        IStockRepository stockRepository,
        IHttpContextAccessor httpContextAccessor)
    {
        _stockRepository = stockRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Guid> Handle(CreateStockCommand request, CancellationToken cancellationToken)
    {
        // 1. Get organization ID from claims
        var orgIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("OrganizationId")?.Value;
        if (string.IsNullOrEmpty(orgIdClaim) || !Guid.TryParse(orgIdClaim, out var organizationId))
        {
            throw new UnauthorizedException("معرف المؤسسة مطلوب");
        }

        // 2. Check if stock already exists for this packaging/inventory combination
        var existingStock = await _stockRepository.GetByPackagingAsync(
            request.ProductPackagingId,
            organizationId,
            request.InventoryId,
            cancellationToken);

        if (existingStock != null)
        {
            throw new ValidationException("المخزون موجود بالفعل لهذا المنتج في هذا المخزن");
        }

        // 3. Create stock using domain factory
        var stock = Stock.Create(
            productPackagingId: request.ProductPackagingId,
            organizationId: organizationId,
            inventoryId: request.InventoryId
        );

        // 4. Add initial batch if provided
        if (!string.IsNullOrWhiteSpace(request.InitialBatchNumber) && request.InitialQuantity.HasValue)
        {
            stock.AddBatch(
                batchNumber: request.InitialBatchNumber,
                quantity: request.InitialQuantity.Value,
                expiryDate: request.InitialExpiryDate,
                condition: request.InitialCondition ?? StockCondition.New,
                costPrice: request.InitialCostPrice
            );
        }

        // 5. Persist changes
        await _stockRepository.AddAsync(stock, cancellationToken);
        await _stockRepository.SaveChangesAsync(cancellationToken);

        return stock.Id;
    }
}
