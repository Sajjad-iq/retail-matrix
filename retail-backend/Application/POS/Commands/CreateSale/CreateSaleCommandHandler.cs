using Application.Common.Exceptions;
using Application.Common.Services;
using Domains.Common.Currency.Services;
using Domains.Inventory.Repositories;
using Domains.Products.Repositories;
using Domains.Sales.Entities;
using Domains.Sales.Repositories;
using Domains.Shared.ValueObjects;
using Domains.Stocks.Repositories;
using Infrastructure.Data;
using MediatR;

namespace Application.POS.Commands.CreateSale;

public class CreateSaleCommandHandler : IRequestHandler<CreateSaleCommand, Guid>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IProductPackagingRepository _productPackagingRepository;
    private readonly IStockRepository _stockRepository;
    private readonly ICurrencyConversionService _currencyService;
    private readonly IOrganizationContext _organizationContext;
    private readonly IInventoryRepository _inventoryRepository;
    private readonly ApplicationDbContext _dbContext;

    public CreateSaleCommandHandler(
        ISaleRepository saleRepository,
        IProductPackagingRepository productPackagingRepository,
        IStockRepository stockRepository,
        ICurrencyConversionService currencyService,
        IOrganizationContext organizationContext,
        IInventoryRepository inventoryRepository,
        ApplicationDbContext dbContext)
    {
        _saleRepository = saleRepository;
        _productPackagingRepository = productPackagingRepository;
        _stockRepository = stockRepository;
        _currencyService = currencyService;
        _organizationContext = organizationContext;
        _inventoryRepository = inventoryRepository;
        _dbContext = dbContext;
    }

    public async Task<Guid> Handle(CreateSaleCommand request, CancellationToken cancellationToken)
    {
        // Extract organization ID and user ID from context
        var organizationId = _organizationContext.OrganizationId;
        var userId = _organizationContext.UserId;

        // Validate inventory exists and belongs to organization
        var inventory = await _inventoryRepository.GetByIdAsync(request.InventoryId, cancellationToken);
        if (inventory == null || inventory.OrganizationId != organizationId)
        {
            throw new NotFoundException("المخزن غير موجود");
        }

        // Begin transaction to ensure atomicity
        using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            // Create a new draft sale
            var sale = Sale.Create(
                organizationId: organizationId,
                salesPersonId: userId
            );

            // Add notes if provided
            if (!string.IsNullOrWhiteSpace(request.Notes))
            {
                sale.AddNotes(request.Notes);
            }

            // Add items if provided
            foreach (var itemInput in request.Items)
            {
                await AddItemToSaleAsync(sale, itemInput, organizationId, request.InventoryId, cancellationToken);
            }

            // Persist the sale
            await _saleRepository.AddAsync(sale, cancellationToken);
            await _saleRepository.SaveChangesAsync(cancellationToken);

            // Commit transaction
            await transaction.CommitAsync(cancellationToken);

            return sale.Id;
        }
        catch
        {
            // Transaction will be rolled back automatically on exception
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private async Task AddItemToSaleAsync(
        Sale sale,
        SaleItemInput itemInput,
        Guid organizationId,
        Guid inventoryId,
        CancellationToken cancellationToken)
    {
        // Get product packaging by barcode or ID with Product navigation property loaded
        var packaging = itemInput.ProductPackagingId.HasValue
            ? await _productPackagingRepository.GetByIdWithProductAsync(itemInput.ProductPackagingId.Value, cancellationToken)
            : !string.IsNullOrWhiteSpace(itemInput.Barcode)
                ? await _productPackagingRepository.GetByBarcodeWithProductAsync(itemInput.Barcode, cancellationToken)
                : null;

        if (packaging == null)
        {
            var identifier = itemInput.Barcode ?? itemInput.ProductPackagingId?.ToString() ?? "غير معروف";
            throw new NotFoundException($"المنتج غير موجود: {identifier}");
        }

        // Validate organization ownership
        if (packaging.Product?.OrganizationId != organizationId)
        {
            throw new UnauthorizedException("المنتج لا ينتمي لمؤسستك");
        }

        // Get stock with batches loaded for reservation
        var stock = await _stockRepository.GetByPackagingWithBatchesAsync(
            packaging.Id,
            organizationId,
            inventoryId,
            cancellationToken);

        if (stock == null || stock.TotalAvailableQuantity < itemInput.Quantity)
        {
            var available = stock?.TotalAvailableQuantity ?? 0;
            throw new ValidationException($"الكمية المتاحة في المخزون ({available}) أقل من المطلوبة ({itemInput.Quantity}) للمنتج: {packaging.Name}");
        }

        // Reserve stock using FEFO (First Expired, First Out) strategy
        var remainingToReserve = itemInput.Quantity;
        foreach (var batch in stock.GetAvailableBatches())
        {
            if (remainingToReserve <= 0)
                break;

            var quantityToReserve = Math.Min(remainingToReserve, batch.AvailableQuantity);
            batch.Reserve(quantityToReserve);
            remainingToReserve -= quantityToReserve;
        }

        // Get product name
        var productName = packaging.Product?.Name ?? packaging.Name;

        // Calculate discount if provided
        Discount? discount = null;
        if (itemInput.Discount != null)
        {
            discount = itemInput.Discount.IsPercentage
                ? Discount.Percentage(itemInput.Discount.Amount)
                : Discount.FixedAmount(itemInput.Discount.Amount);
        }

        // Add item to sale
        await sale.AddItemAsync(
            productPackagingId: packaging.Id,
            productName: productName,
            quantity: itemInput.Quantity,
            unitPrice: packaging.GetDiscountedPrice(),
            currencyService: _currencyService,
            discount: discount,
            cancellationToken: cancellationToken
        );
    }
}
