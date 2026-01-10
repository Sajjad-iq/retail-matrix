using Domains.Entities;
using Domains.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

/// <summary>
/// Entity Framework Core implementation of IProductPackagingRepository
/// </summary>
public class ProductPackagingRepository : IProductPackagingRepository
{
    private readonly ApplicationDbContext _context;

    public ProductPackagingRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ProductPackaging?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ProductPackagings
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<ProductPackaging?> GetByBarcodeAsync(string barcode, CancellationToken cancellationToken = default)
    {
        return await _context.ProductPackagings
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Barcode == barcode, cancellationToken);
    }

    public async Task<List<ProductPackaging>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await _context.ProductPackagings
            .AsNoTracking()
            .Where(p => p.ProductId == productId)
            .OrderByDescending(p => p.IsDefault)
            .ThenBy(p => p.UnitOfMeasure)
            .ToListAsync(cancellationToken);
    }

    public async Task<ProductPackaging?> GetDefaultPackagingAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await _context.ProductPackagings
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.ProductId == productId && p.IsDefault, cancellationToken);
    }

    public async Task<List<ProductPackaging>> GetByOrganizationAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        return await _context.ProductPackagings
            .AsNoTracking()
            .Where(p => p.OrganizationId == organizationId)
            .OrderBy(p => p.ProductId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<ProductPackaging>> GetActivePackagingsAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await _context.ProductPackagings
            .AsNoTracking()
            .Where(p => p.ProductId == productId && p.IsActive)
            .OrderByDescending(p => p.IsDefault)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByBarcodeAsync(string barcode, CancellationToken cancellationToken = default)
    {
        return await _context.ProductPackagings
            .AsNoTracking()
            .AnyAsync(p => p.Barcode == barcode, cancellationToken);
    }

    public async Task<ProductPackaging> AddAsync(ProductPackaging packaging, CancellationToken cancellationToken = default)
    {
        await _context.ProductPackagings.AddAsync(packaging, cancellationToken);
        return packaging;
    }

    public async Task<ProductPackaging> UpdateAsync(ProductPackaging packaging, CancellationToken cancellationToken = default)
    {
        _context.ProductPackagings.Update(packaging);
        return await Task.FromResult(packaging);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var packaging = await _context.ProductPackagings.FindAsync(new object[] { id }, cancellationToken);

        if (packaging == null)
            return false;

        _context.ProductPackagings.Remove(packaging); // Soft delete via SaveChangesAsync override
        return true;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
