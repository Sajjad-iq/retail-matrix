using Domains.Enums;
using Domains.Shared;
using Domains.ValueObjects;

namespace Domains.Entities;

/// <summary>
/// Organization aggregate root representing a business entity
/// </summary>
public class Organization : BaseEntity
{
    // Parameterless constructor for EF Core
    private Organization()
    {
        Name = string.Empty;
        Domain = string.Empty;
        Description = string.Empty;
        Address = string.Empty;
        Phone = string.Empty;
    }

    // Private constructor to enforce factory methods
    private Organization(
        string name,
        string domain,
        string phone,
        Guid createdBy,
        string? description = null,
        string? address = null,
        string? logoUrl = null)
    {
        Id = Guid.NewGuid();
        Name = name;
        Domain = domain;
        Phone = phone;
        CreatedBy = createdBy;
        Description = description ?? string.Empty;
        Address = address ?? string.Empty;
        LogoUrl = logoUrl;
        Status = OrganizationStatus.Active;
        InsertDate = DateTime.UtcNow;
        UpdateDate = DateTime.UtcNow;
    }

    // Properties with proper encapsulation
    public string Name { get; private set; }
    public string Domain { get; private set; }
    public string Description { get; private set; }
    public string Address { get; private set; }
    public string Phone { get; private set; }
    public OrganizationStatus Status { get; private set; }
    public Guid CreatedBy { get; private set; }
    public string? LogoUrl { get; private set; }

    /// <summary>
    /// Factory method to create a new organization
    /// </summary>
    public static Organization Create(
        string name,
        string domain,
        string phone,
        Guid createdBy,
        string? description = null,
        string? address = null,
        string? logoUrl = null)
    {
        // Validate using value objects
        var orgName = ValueObjects.Name.Create(name, minLength: 2, maxLength: 200);
        var orgDomain = ValueObjects.DomainName.Create(domain);
        var orgPhone = ValueObjects.Phone.Create(phone);

        if (createdBy == Guid.Empty)
            throw new ArgumentException("معرف المنشئ مطلوب", nameof(createdBy));

        return new Organization(
            name: orgName,
            domain: orgDomain,
            phone: orgPhone,
            createdBy: createdBy,
            description: description?.Trim(),
            address: address?.Trim(),
            logoUrl: logoUrl
        );
    }

    // Domain Methods
    public void UpdateProfile(
        string name,
        string? description = null,
        string? address = null,
        string? logoUrl = null)
    {
        var orgName = ValueObjects.Name.Create(name, minLength: 2, maxLength: 200);

        Name = orgName;
        Description = description?.Trim() ?? Description;
        Address = address?.Trim() ?? Address;
        LogoUrl = logoUrl ?? LogoUrl;
        UpdateDate = DateTime.UtcNow;
    }

    public void UpdateDomain(string domain)
    {
        var orgDomain = DomainName.Create(domain);
        Domain = orgDomain;
        UpdateDate = DateTime.UtcNow;
    }

    public void UpdatePhone(string phone)
    {
        var orgPhone = ValueObjects.Phone.Create(phone);
        Phone = orgPhone;
        UpdateDate = DateTime.UtcNow;
    }

    public void UpdateLogo(string logoUrl)
    {
        if (string.IsNullOrWhiteSpace(logoUrl))
            throw new ArgumentException("رابط الشعار مطلوب", nameof(logoUrl));

        LogoUrl = logoUrl;
        UpdateDate = DateTime.UtcNow;
    }

    public void Activate()
    {
        if (Status == OrganizationStatus.Closed)
            throw new InvalidOperationException("لا يمكن تفعيل مؤسسة مغلقة");

        Status = OrganizationStatus.Active;
        UpdateDate = DateTime.UtcNow;
    }

    public void Suspend()
    {
        if (Status == OrganizationStatus.Closed)
            throw new InvalidOperationException("لا يمكن تعليق مؤسسة مغلقة");

        Status = OrganizationStatus.Suspended;
        UpdateDate = DateTime.UtcNow;
    }

    public void Close()
    {
        Status = OrganizationStatus.Closed;
        UpdateDate = DateTime.UtcNow;
    }

    public void ApprovePending()
    {
        if (Status != OrganizationStatus.Pending)
            throw new InvalidOperationException("المؤسسة ليست في حالة الانتظار");

        Status = OrganizationStatus.Active;
        UpdateDate = DateTime.UtcNow;
    }

    public bool IsActive() => Status == OrganizationStatus.Active;

    public bool IsSuspended() => Status == OrganizationStatus.Suspended;

    public bool IsClosed() => Status == OrganizationStatus.Closed;

    public bool IsPending() => Status == OrganizationStatus.Pending;
}
