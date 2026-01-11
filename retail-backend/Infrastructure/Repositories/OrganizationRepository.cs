using Domains.Organizations.Entities;
using Domains.Organizations.Repositories;
using Domains.Shared.Base;
using Domains.Sales.Enums;
using Domains.Products.Enums;
using Domains.Organizations.Enums;
using Domains.Users.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

/// <summary>
/// Entity Framework Core implementation of IOrganizationRepository
/// </summary>
public class OrganizationRepository : Repository<Organization>, IOrganizationRepository
{
    public OrganizationRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Organization?> GetByDomainAsync(string domain, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Domain == domain.ToLowerInvariant(), cancellationToken);
    }

    public async Task<List<Organization>> GetByStatusAsync(OrganizationStatus status, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(o => o.Status == status)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Organization>> GetByCreatorAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(o => o.CreatedBy == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Organization>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(o => o.Status == OrganizationStatus.Active)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Organization>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByDomainAsync(string domain, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .AnyAsync(o => o.Domain == domain.ToLowerInvariant(), cancellationToken);
    }
}
