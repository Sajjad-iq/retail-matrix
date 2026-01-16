using Domains.Inventory.Enums;
using Domains.Shared.Base;

namespace Domains.Inventory.Entities;

/// <summary>
/// Represents a storage location (bin, shelf, drawer) within a warehouse
/// </summary>
public class Inventory : BaseEntity
{
    // Parameterless constructor for EF Core
    private Inventory()
    {
        Name = string.Empty;
        Code = string.Empty;
    }

    // Private constructor to enforce factory methods
    private Inventory(
        string name,
        string code,
        InventoryType type,
        Guid organizationId,
        Guid? parentId = null)
    {
        Id = Guid.NewGuid();
        Name = name;
        Code = code;
        Type = type;
        OrganizationId = organizationId;
        ParentId = parentId;
        IsActive = true;
        InsertDate = DateTime.UtcNow;
    }

    // Properties
    public string Name { get; private set; }        // e.g., "Aisle A", "Shelf 3", "Bin 5"
    public string Code { get; private set; }        // e.g., "A-3-5" for scanning
    public InventoryType Type { get; private set; }
    public Guid OrganizationId { get; private set; }
    public Guid? ParentId { get; private set; }     // Hierarchical: Aisle > Rack > Shelf > Bin
    public bool IsActive { get; private set; }

    // Navigation properties
    public Inventory? Parent { get; private set; }
    public List<Inventory> Children { get; private set; } = new();

    /// <summary>
    /// Factory method to create a new storage inventory
    /// </summary>
    public static Inventory Create(
        string name,
        string code,
        InventoryType type,
        Guid organizationId,
        Guid? parentId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("اسم الموقع مطلوب", nameof(name));

        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("رمز الموقع مطلوب", nameof(code));

        if (organizationId == Guid.Empty)
            throw new ArgumentException("معرف المؤسسة مطلوب", nameof(organizationId));

        return new Inventory(name, code, type, organizationId, parentId);
    }

    // Update methods
    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("اسم الموقع مطلوب", nameof(name));

        Name = name;
    }

    public void UpdateCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("رمز الموقع مطلوب", nameof(code));

        // Note: Code uniqueness per organization should be validated at the application/repository level
        Code = code;
    }

    public void UpdateType(InventoryType type)
    {
        Type = type;
    }

    public void SetParent(Guid? parentId)
    {
        ParentId = parentId;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    /// <summary>
    /// Get full path code (e.g., "A-3-5" from hierarchy)
    /// </summary>
    public string GetFullPath()
    {
        if (Parent == null)
            return Code;

        return $"{Parent.GetFullPath()}-{Code}";
    }
}
