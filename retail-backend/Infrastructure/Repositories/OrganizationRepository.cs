using Domains.Entities;
using Domains.Enums;
using Domains.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

/// <summary>
/// Entity Framework Core implementation of IOrganizationRepository
/// </summary>
public class OrganizationRepository : IOrganizationRepository
{
    private readonly ApplicationDbContext _context;

    public OrganizationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Organization?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Organizations
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public async Task<Organization?> GetByDomainAsync(string domain, CancellationToken cancellationToken = default)
    {
        return await _context.Organizations
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Domain == domain.ToLowerInvariant(), cancellationToken);
    }

    public async Task<List<Organization>> GetByStatusAsync(OrganizationStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.Organizations
            .AsNoTracking()
            .Where(o => o.Status == status)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Organization>> GetByCreatorAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Organizations
            .AsNoTracking()
            .Where(o => o.CreatedBy == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Organization>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Organizations
            .AsNoTracking()
            .Where(o => o.Status == OrganizationStatus.Active)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Organization>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Organizations
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByDomainAsync(string domain, CancellationToken cancellationToken = default)
    {
        return await _context.Organizations
            .AsNoTracking()
            .AnyAsync(o => o.Domain == domain.ToLowerInvariant(), cancellationToken);
    }

    public async Task<Organization> AddAsync(Organization organization, CancellationToken cancellationToken = default)
    {
        await _context.Organizations.AddAsync(organization, cancellationToken);
        return organization;
    }

    public async Task<Organization> UpdateAsync(Organization organization, CancellationToken cancellationToken = default)
    {
        _context.Organizations.Update(organization);
        return await Task.FromResult(organization);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var organization = await _context.Organizations.FindAsync(new object[] { id }, cancellationToken);

        if (organization == null)
            return false;

        _context.Organizations.Remove(organization); // Soft delete via SaveChangesAsync override
        return true;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
