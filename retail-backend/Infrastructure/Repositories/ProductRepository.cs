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
        return await _context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Barcode == barcode, cancellationToken);
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
        return await _context.Products
            .AsNoTracking()
            .Where(p => p.OrganizationId == organizationId
                     && p.CurrentStock > 0
                     && p.CurrentStock <= p.ReorderLevel)
            .OrderBy(p => p.CurrentStock)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Product>> GetOutOfStockProductsAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .AsNoTracking()
            .Where(p => p.OrganizationId == organizationId && p.CurrentStock == 0)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }


    public async Task<bool> ExistsByBarcodeAsync(string barcode, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .AsNoTracking()
            .AnyAsync(p => p.Barcode == barcode, cancellationToken);
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
