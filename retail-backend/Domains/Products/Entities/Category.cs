using Domains.Shared.Base;
using NameVO = Domains.Users.ValueObjects.Name;

namespace Domains.Products.Entities;

/// <summary>
/// Category entity for product classification with hierarchy support
/// </summary>
public class Category : BaseEntity
{
    // Parameterless constructor for EF Core
    private Category()
    {
        Name = string.Empty;
        Description = string.Empty;
        SubCategories = new List<Category>();
    }

    // Private constructor to enforce factory methods
    private Category(
        string name,
        Guid organizationId,
        Guid? parentCategoryId = null,
        string? description = null)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description ?? string.Empty;
        ParentCategoryId = parentCategoryId;
        OrganizationId = organizationId;
        IsActive = true;
        SubCategories = new List<Category>();
        InsertDate = DateTime.UtcNow;
    }

    // Properties
    public string Name { get; private set; }
    public string Description { get; private set; }
    public Guid? ParentCategoryId { get; private set; }
    public Guid OrganizationId { get; private set; }
    public bool IsActive { get; private set; }

    // Navigation properties
    public Category? ParentCategory { get; private set; }
    public List<Category> SubCategories { get; private set; }

    /// <summary>
    /// Factory method to create a new category
    /// </summary>
    public static Category Create(
        string name,
        Guid organizationId,
        Guid? parentCategoryId = null,
        string? description = null)
    {
        // Validate using value object
        var categoryName = NameVO.Create(name, minLength: 2, maxLength: 100);

        if (organizationId == Guid.Empty)
            throw new ArgumentException("معرف المؤسسة مطلوب", nameof(organizationId));

        return new Category(
            name: categoryName,
            organizationId: organizationId,
            parentCategoryId: parentCategoryId,
            description: description?.Trim()
        );
    }

    // Domain Methods
    public void UpdateInfo(string name, string? description = null)
    {
        var categoryName = NameVO.Create(name, minLength: 2, maxLength: 100);

        Name = categoryName;
        Description = description?.Trim() ?? Description;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void AssignParent(Guid parentCategoryId)
    {
        if (parentCategoryId == Id)
            throw new InvalidOperationException("لا يمكن تعيين الفئة كأب لنفسها");

        ParentCategoryId = parentCategoryId;
    }

    public void RemoveParent()
    {
        ParentCategoryId = null;
    }
}
