using Domains.Products.Entities;
using Domains.Shared.Base;

namespace Domains.Products.Repositories;

/// <summary>
/// Repository interface for Category entity
/// </summary>
public interface ICategoryRepository : IRepository<Category>
{
    Task<Category?> GetByNameAsync(string name, Guid organizationId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Category>> GetByOrganizationAsync(Guid organizationId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Category>> GetActiveAsync(Guid organizationId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Category>> GetRootCategoriesAsync(Guid organizationId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Category>> GetSubCategoriesAsync(Guid parentCategoryId, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(string name, Guid organizationId, CancellationToken cancellationToken = default);
    Task<bool> HasSubCategoriesAsync(Guid categoryId, CancellationToken cancellationToken = default);
}
