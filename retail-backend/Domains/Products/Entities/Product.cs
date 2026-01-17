using Domains.Products.Enums;
using Domains.Shared.Base;
using Domains.Shared.ValueObjects;
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
        Packagings = new List<ProductPackaging>();
    }

    // Private constructor to enforce factory methods
    private Product(
        Guid organizationId)
    {
        Id = Guid.NewGuid();
        OrganizationId = organizationId;
        Status = ProductStatus.Active;
        Packagings = new List<ProductPackaging>();
        InsertDate = DateTime.UtcNow;
    }

    // Properties with proper encapsulation
    public ProductStatus Status { get; private set; }
    public Guid OrganizationId { get; private set; }
    public Guid? CategoryId { get; private set; }

    // Navigation properties
    public Category? Category { get; private set; }
    public List<ProductPackaging> Packagings { get; private set; }

    /// <summary>
    /// Factory method to create a new product
    /// </summary>
    public static Product Create(
        Guid organizationId)
    {
        if (organizationId == Guid.Empty)
            throw new ArgumentException("معرف المؤسسة مطلوب", nameof(organizationId));

        return new Product(
            organizationId: organizationId
        );
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

    public void AssignCategory(Guid categoryId)
    {
        if (categoryId == Guid.Empty)
            throw new ArgumentException("معرف الفئة غير صالح", nameof(categoryId));

        CategoryId = categoryId;
    }

    public void UnassignCategory()
    {
        CategoryId = null;
    }

    public ProductPackaging AddPackaging(
        string name,
        Price sellingPrice,
        UnitOfMeasure unitOfMeasure,
        string? barcode = null,
        string? description = null,
        int unitsPerPackage = 1,
        bool isDefault = false,
        List<string>? imageUrls = null,
        string? dimensions = null,
        Weight? weight = null,
        string? color = null)
    {
        var packaging = ProductPackaging.Create(
            productId: Id,
            name: name,
            sellingPrice: sellingPrice,
            unitOfMeasure: unitOfMeasure,
            barcode: barcode,
            description: description,
            unitsPerPackage: unitsPerPackage,
            isDefault: isDefault,
            imageUrls: imageUrls,
            dimensions: dimensions,
            weight: weight,
            color: color
        );

        Packagings.Add(packaging);
        return packaging;
    }

    public void RemovePackaging(Guid packagingId)
    {
        var packaging = Packagings.FirstOrDefault(p => p.Id == packagingId);
        if (packaging == null)
            throw new InvalidOperationException("العبوة غير موجودة");

        Packagings.Remove(packaging);
    }
}
