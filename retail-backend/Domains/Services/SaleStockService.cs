using Domains.Entities;
using Domains.Repositories;

namespace Domains.Services;

/// <summary>
/// Domain service for managing stock operations related to sales
/// </summary>
public class SaleStockService
{
    private readonly IProductStockRepository _stockRepository;

    public SaleStockService(IProductStockRepository stockRepository)
    {
        _stockRepository = stockRepository;
    }

    /// <summary>
    /// Reserve stock for a draft sale
    /// </summary>
    public async Task ReserveStockForSale(Sale sale, CancellationToken cancellationToken = default)
    {
        foreach (var item in sale.Items)
        {
            var stock = await _stockRepository.GetByPackagingAndLocationAsync(
                item.ProductPackagingId,
                sale.LocationId,
                cancellationToken
            );

            if (stock == null)
                throw new InvalidOperationException($"لا يوجد مخزون للمنتج {item.ProductName}");

            stock.ReserveStock(item.Quantity);
            await _stockRepository.UpdateAsync(stock, cancellationToken);
        }

        await _stockRepository.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Deduct stock when sale is completed
    /// </summary>
    public async Task DeductStockForCompletedSale(Sale sale, CancellationToken cancellationToken = default)
    {
        foreach (var item in sale.Items)
        {
            var stock = await _stockRepository.GetByPackagingAndLocationAsync(
                item.ProductPackagingId,
                sale.LocationId,
                cancellationToken
            );

            if (stock == null)
                throw new InvalidOperationException($"لا يوجد مخزون للمنتج {item.ProductName}");

            // Deduct stock using negative quantity
            stock.AdjustStock(-item.Quantity, $"Sale {sale.SaleNumber}");
            await _stockRepository.UpdateAsync(stock, cancellationToken);
        }

        await _stockRepository.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Release reserved stock when sale is cancelled
    /// </summary>
    public async Task ReleaseReservedStock(Sale sale, CancellationToken cancellationToken = default)
    {
        foreach (var item in sale.Items)
        {
            var stock = await _stockRepository.GetByPackagingAndLocationAsync(
                item.ProductPackagingId,
                sale.LocationId,
                cancellationToken
            );

            if (stock != null)
            {
                stock.ReleaseReservedStock(item.Quantity);
                await _stockRepository.UpdateAsync(stock, cancellationToken);
            }
        }

        await _stockRepository.SaveChangesAsync(cancellationToken);
    }
}
