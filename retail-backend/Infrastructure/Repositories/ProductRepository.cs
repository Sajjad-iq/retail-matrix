using Domains.Entities;
using Domains.Enums;
using Domains.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

/// <summary>
/// Entity Framework Core implementation of IProductRepository
/// </summary>
public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;

    public ProductRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Product?> GetByBarcodeAsync(string barcode, CancellationToken cancellationToken = default)
    {
        // Barcode is now in ProductPackaging, not Product
        throw new NotSupportedException("Use IProductPackagingRepository.GetByBarcodeAsync instead");
    }

    public async Task<List<Product>> GetByOrganizationAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .AsNoTracking()
            .Where(p => p.OrganizationId == organizationId)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Product>> GetByStatusAsync(ProductStatus status, Guid organizationId, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .AsNoTracking()
            .Where(p => p.Status == status && p.OrganizationId == organizationId)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Product>> GetLowStockProductsAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        // Stock is now in ProductStock, not Product
        throw new NotSupportedException("Use IProductStockRepository.GetLowStockItemsAsync instead");
    }

    public async Task<List<Product>> GetOutOfStockProductsAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        // Stock is now in ProductStock, not Product
        throw new NotSupportedException("Use IProductStockRepository.GetOutOfStockItemsAsync instead");
    }

    public async Task<bool> ExistsByBarcodeAsync(string barcode, CancellationToken cancellationToken = default)
    {
        // Barcode is now in ProductPackaging, not Product
        throw new NotSupportedException("Use IProductPackagingRepository.ExistsByBarcodeAsync instead");
    }

    public async Task<Product> AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        await _context.Products.AddAsync(product, cancellationToken);
        return product;
    }

    public async Task<Product> UpdateAsync(Product product, CancellationToken cancellationToken = default)
    {
        _context.Products.Update(product);
        return await Task.FromResult(product);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _context.Products.FindAsync(new object[] { id }, cancellationToken);

        if (product == null)
            return false;

        _context.Products.Remove(product); // Soft delete via SaveChangesAsync override
        return true;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
