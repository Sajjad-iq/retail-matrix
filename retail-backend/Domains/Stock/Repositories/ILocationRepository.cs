using Domains.Stock.Entities;
using Domains.Stock.Enums;
using Domains.Shared.Base;

namespace Domains.Stock.Repositories;

/// <summary>
/// Repository interface for Location entity (storage spots)
/// </summary>
public interface ILocationRepository : IRepository<Location>
{
    Task<Location?> GetByCodeAsync(string code, Guid organizationId, CancellationToken cancellationToken = default);
    
    Task<PagedResult<Location>> GetByOrganizationAsync(
        Guid organizationId,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default);
    
    Task<PagedResult<Location>> GetByTypeAsync(
        Guid organizationId,
        LocationType type,
        PagingParams pagingParams,
        CancellationToken cancellationToken = default);
    
    Task<List<Location>> GetChildrenAsync(
        Guid parentId,
        CancellationToken cancellationToken = default);
    
    Task<List<Location>> GetRootLocationsAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default);
    
    Task<List<Location>> GetActiveLocationsAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default);
}
