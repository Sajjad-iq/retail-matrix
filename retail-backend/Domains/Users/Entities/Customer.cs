using Domains.Shared.Base;

namespace Domains.Users.Entities;

/// <summary>
/// Represents a customer who can make purchases and have installment plans
/// </summary>
public class Customer : BaseEntity
{
    // Parameterless constructor for EF Core
    private Customer()
    {
        Name = string.Empty;
        PhoneNumber = string.Empty;
    }

    // Private constructor to enforce factory methods
    private Customer(
        Guid organizationId,
        string name,
        string phoneNumber,
        string? email = null,
        string? address = null,
        string? notes = null)
    {
        Id = Guid.NewGuid();
        OrganizationId = organizationId;
        Name = name;
        PhoneNumber = phoneNumber;
        Email = email;
        Address = address;
        Notes = notes;
        InsertDate = DateTime.UtcNow;
    }

    // Properties
    public Guid OrganizationId { get; private set; }
    public string Name { get; private set; }
    public string PhoneNumber { get; private set; }
    public string? Email { get; private set; }
    public string? Address { get; private set; }
    public string? Notes { get; private set; }

    /// <summary>
    /// Factory method to create a new customer
    /// </summary>
    public static Customer Create(
        Guid organizationId,
        string name,
        string phoneNumber,
        string? email = null,
        string? address = null,
        string? notes = null)
    {
        if (organizationId == Guid.Empty)
            throw new ArgumentException("معرف المؤسسة مطلوب", nameof(organizationId));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("اسم العميل مطلوب", nameof(name));

        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("رقم الهاتف مطلوب", nameof(phoneNumber));

        return new Customer(
            organizationId: organizationId,
            name: name,
            phoneNumber: phoneNumber,
            email: email,
            address: address,
            notes: notes
        );
    }

    // Domain Methods
    public void UpdateInfo(
        string name,
        string phoneNumber,
        string? email = null,
        string? address = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("اسم العميل مطلوب", nameof(name));

        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("رقم الهاتف مطلوب", nameof(phoneNumber));

        Name = name;
        PhoneNumber = phoneNumber;
        Email = email;
        Address = address;
    }

    public void UpdateNotes(string? notes)
    {
        Notes = notes;
    }
}
