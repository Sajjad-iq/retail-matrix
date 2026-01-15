using Domains.Inventory.Entities;
using Domains.Products.Entities;
using Domains.Inventory.Enums;
using Domains.Inventory.Repositories;
using Domains.Shared.Base;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class InventoryMovementRepository : Repository<InventoryMovement>, IInventoryMovementRepository
{
    public InventoryMovementRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<InventoryMovement>> GetByPackagingAsync(Guid packagingId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(m => m.ProductPackagingId == packagingId)
            .OrderByDescending(m => m.MovementDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<InventoryMovement>> GetByDateRangeAsync(Guid organizationId, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(m => m.OrganizationId == organizationId &&
                       m.MovementDate >= fromDate &&
                       m.MovementDate <= toDate)
            .OrderByDescending(m => m.MovementDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<InventoryMovement>> GetByTypeAsync(Guid organizationId, InventoryMovementType type, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(m => m.OrganizationId == organizationId && m.Type == type)
            .OrderByDescending(m => m.MovementDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<InventoryMovement>> GetByReferenceNumberAsync(string referenceNumber, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(m => m.ReferenceNumber == referenceNumber)
            .OrderByDescending(m => m.MovementDate)
            .ToListAsync(cancellationToken);
    }
}
