using Domains.Stock.Entities;
using Domains.Stock.Enums;
using Domains.Stock.Repositories;
using Domains.Shared.Base;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

/// <summary>
/// Entity Framework Core implementation of ILocationRepository
/// </summary>
public class LocationRepository : Repository<Location>, ILocationRepository
{
    public LocationRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Location?> GetByCodeAsync(
        string code,
        Guid organizationId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Code == code && l.OrganizationId == organizationId, cancellationToken);
    }

    public async Task<PagedResult<Location>> GetByOrganizationAsync(
        Guid organizationId,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .AsNoTracking()
            .Where(l => l.OrganizationId == organizationId)
            .OrderBy(l => l.Code);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip(pagingParams.Skip)
            .Take(pagingParams.Take)
            .ToListAsync(cancellationToken);

        return new PagedResult<Location>(items, totalCount, pagingParams.PageNumber, pagingParams.PageSize);
    }

    public async Task<PagedResult<Location>> GetByTypeAsync(
        Guid organizationId,
        LocationType type,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .AsNoTracking()
            .Where(l => l.OrganizationId == organizationId && l.Type == type)
            .OrderBy(l => l.Code);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip(pagingParams.Skip)
            .Take(pagingParams.Take)
            .ToListAsync(cancellationToken);

        return new PagedResult<Location>(items, totalCount, pagingParams.PageNumber, pagingParams.PageSize);
    }

    public async Task<List<Location>> GetChildrenAsync(
        Guid parentId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(l => l.ParentId == parentId)
            .OrderBy(l => l.Code)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Location>> GetRootLocationsAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(l => l.OrganizationId == organizationId && l.ParentId == null)
            .OrderBy(l => l.Code)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Location>> GetActiveLocationsAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(l => l.OrganizationId == organizationId && l.IsActive)
            .OrderBy(l => l.Code)
            .ToListAsync(cancellationToken);
    }
}
