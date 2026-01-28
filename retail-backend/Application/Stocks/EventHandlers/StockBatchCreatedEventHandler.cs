using Application.Common.Services;
using Domains.Inventory.Entities;
using Domains.Inventory.Enums;
using Domains.Inventory.Repositories;
using Domains.Products.Repositories;
using Domains.Shared.ValueObjects;
using Domains.Stocks.Events;
using MediatR;

namespace Application.Stocks.EventHandlers;

/// <summary>
/// Handles StockBatchCreatedEvent by creating an inventory operation for auditing
/// </summary>
public class StockBatchCreatedEventHandler : INotificationHandler<StockBatchCreatedEvent>
{
    private readonly IInventoryOperationRepository _operationRepository;
    private readonly IProductPackagingRepository _packagingRepository;
    private readonly IOrganizationContext _organizationContext;

    public StockBatchCreatedEventHandler(
        IInventoryOperationRepository operationRepository,
        IProductPackagingRepository packagingRepository,
        IOrganizationContext organizationContext)
    {
        _operationRepository = operationRepository;
        _packagingRepository = packagingRepository;
        _organizationContext = organizationContext;
    }

    public async Task Handle(StockBatchCreatedEvent notification, CancellationToken cancellationToken)
    {
        // Get product packaging for product name
        var packaging = await _packagingRepository.GetByIdAsync(notification.ProductPackagingId, cancellationToken);
        if (packaging == null)
        {
            // Log warning but don't fail - auditing shouldn't block stock operations
            return;
        }

        var productName = packaging.Product?.Name ?? "Unknown Product";
        var barcode = packaging.Barcode;

        // Create inventory operation for audit trail
        var operation = InventoryOperation.Create(
            operationType: InventoryOperationType.Adjustment,
            operationNumber: $"BATCH-{notification.BatchId.ToString()[..8]}",
            userId: _organizationContext.UserId,
            organizationId: notification.OrganizationId,
            sourceInventoryId: notification.InventoryId,
            notes: $"Stock batch created: {notification.BatchNumber}"
        );

        // Create operation item
        var item = InventoryOperationItem.Create(
            inventoryOperationId: operation.Id,
            productPackagingId: notification.ProductPackagingId,
            productName: productName,
            barcode: barcode ?? string.Empty,
            quantity: notification.Quantity,
            unitPrice: notification.CostPrice ?? new Price(0, "IQD"),
            notes: notification.ExpiryDate.HasValue
                ? $"Expiry: {notification.ExpiryDate.Value:yyyy-MM-dd}"
                : null
        );

        operation.AddItem(item);
        operation.Complete();

        // Persist the audit operation
        await _operationRepository.AddAsync(operation, cancellationToken);
        await _operationRepository.SaveChangesAsync(cancellationToken);
    }
}
