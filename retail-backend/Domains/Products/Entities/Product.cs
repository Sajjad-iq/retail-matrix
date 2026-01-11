using Domains.Products.Enums;
using Domains.Shared.Base;
using NameVO = Domains.Users.ValueObjects.Name;

namespace Domains.Products.Entities;

/// <summary>
/// Product aggregate root representing master product data
/// </summary>
public class Product : BaseEntity
{
    // Parameterless constructor for EF Core
    private Product()
    {
        Name = string.Empty;
        Description = string.Empty;
    }

    // Private constructor to enforce factory methods
    private Product(
        string name,
        Guid organizationId,
        string? description = null,
        string? imageUrl = null)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description ?? string.Empty;
        OrganizationId = organizationId;
        ImageUrl = imageUrl;
        Status = ProductStatus.Active;
        InsertDate = DateTime.UtcNow;
    }

    // Properties with proper encapsulation
    public string Name { get; private set; }
    public string Description { get; private set; }
    public ProductStatus Status { get; private set; }
    public Guid OrganizationId { get; private set; }
    public string? ImageUrl { get; private set; }

    /// <summary>
    /// Factory method to create a new product
    /// </summary>
    public static Product Create(
        string name,
        Guid organizationId,
        string? description = null,
        string? imageUrl = null)
    {
        // Validate using value objects
        var productName = NameVO.Create(name, minLength: 2, maxLength: 200);

        if (organizationId == Guid.Empty)
            throw new ArgumentException("معرف المؤسسة مطلوب", nameof(organizationId));

        return new Product(
            name: productName,
            organizationId: organizationId,
            description: description?.Trim(),
            imageUrl: imageUrl
        );
    }

    // Domain Methods
    public void UpdateBasicInfo(
        string name,
        string? description = null,
        string? imageUrl = null)
    {
        var productName = NameVO.Create(name, minLength: 2, maxLength: 200);

        Name = productName;
        Description = description?.Trim() ?? Description;
        ImageUrl = imageUrl ?? ImageUrl;
    }

    public void Activate()
    {
        if (Status == ProductStatus.Discontinued)
            throw new InvalidOperationException("لا يمكن تفعيل منتج متوقف");

        Status = ProductStatus.Active;
    }

    public void Deactivate()
    {
        Status = ProductStatus.Inactive;
    }

    public void Discontinue()
    {
        Status = ProductStatus.Discontinued;
    }
}
