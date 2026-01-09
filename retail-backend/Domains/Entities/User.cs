using Domains.Enums;
using Domains.Shared;
using Domains.Services;
using Domains.ValueObjects;

namespace Domains.Entities;

public class User : BaseEntity
{
    // Parameterless constructor for EF Core
    private User()
    {
        Name = string.Empty;
        Email = string.Empty;
        PasswordHash = string.Empty;
        PhoneNumber = string.Empty;
        UserRoles = new HashSet<Roles>();
    }

    // Private constructor to enforce factory methods
    private User(
        string name,
        string email,
        string passwordHash,
        string phoneNumber,
        AccountType accountType,
        HashSet<Roles> roles,
        string? address = null,
        string? avatar = null,
        string? memberOfOrganization = null,
        string? department = null)
    {
        Id = Guid.NewGuid();
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
        PhoneNumber = phoneNumber;
        Address = address;
        Avatar = avatar;
        AccountType = accountType;
        UserRoles = roles;
        MemberOfOrganization = memberOfOrganization;
        Department = department;
        IsActive = true;
        EmailVerified = false;
        PhoneVerified = false;
        FailedLoginAttempts = 0;
        LockedUntil = null;
        InsertDate = DateTime.UtcNow;
        UpdateDate = DateTime.UtcNow;
    }

    // Properties with proper encapsulation
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public string PhoneNumber { get; private set; }
    public string? Address { get; private set; }
    public string? Avatar { get; private set; }

    // Account Classification
    public AccountType AccountType { get; private set; }
    public HashSet<Roles> UserRoles { get; private set; }

    // Status & Verification
    public bool IsActive { get; private set; }
    public bool EmailVerified { get; private set; }
    public bool PhoneVerified { get; private set; }

    // Security - Account Locking
    public int FailedLoginAttempts { get; private set; }
    public DateTime? LockedUntil { get; private set; }

    // Business Context
    public string? Department { get; private set; }
    public string? MemberOfOrganization { get; private set; }


    public static User CreateBusinessOwner(
        string name,
        string email,
        string password,
        string phoneNumber,
        IPasswordHasher passwordHasher,
        HashSet<Roles>? roles = null,
        string? address = null,
        string? avatar = null)
    {
        // Validate using value objects
        var userName = ValueObjects.Name.Create(name, minLength: 2, maxLength: 100);
        var userEmail = ValueObjects.Email.Create(email);
        var userPhone = Phone.Create(phoneNumber);
        var validPassword = ValueObjects.Password.Create(password);

        return new User(
            name: userName,
            email: userEmail,
            passwordHash: passwordHasher.HashPassword(password),
            phoneNumber: userPhone,
            accountType: AccountType.BusinessOwner,
            roles: roles ?? new HashSet<Roles> { Roles.Owner },
            address: address,
            avatar: avatar
        );
    }

    public static User CreateEmployee(
        string name,
        string email,
        string password,
        string phoneNumber,
        string memberOfOrganization,
        IPasswordHasher passwordHasher,
        HashSet<Roles>? roles = null,
        string? department = null,
        string? address = null,
        string? avatar = null)
    {
        if (string.IsNullOrWhiteSpace(memberOfOrganization))
            throw new ArgumentException("معرف المؤسسة مطلوب للموظف", nameof(memberOfOrganization));

        // Validate using value objects
        var userName = ValueObjects.Name.Create(name, minLength: 2, maxLength: 100);
        var userEmail = ValueObjects.Email.Create(email);
        var userPhone = Phone.Create(phoneNumber);
        var validPassword = ValueObjects.Password.Create(password);

        return new User(
            name: userName,
            email: userEmail,
            passwordHash: passwordHasher.HashPassword(password),
            phoneNumber: userPhone,
            accountType: AccountType.Employee,
            roles: roles ?? new HashSet<Roles> { Roles.User },
            address: address,
            avatar: avatar,
            memberOfOrganization: memberOfOrganization,
            department: department
        );
    }

    public static User CreateCustomer(
        string name,
        string email,
        string password,
        string phoneNumber,
        IPasswordHasher passwordHasher,
        string? address = null,
        string? avatar = null)
    {
        // Validate using value objects
        var userName = ValueObjects.Name.Create(name, minLength: 2, maxLength: 100);
        var userEmail = ValueObjects.Email.Create(email);
        var userPhone = Phone.Create(phoneNumber);
        var validPassword = ValueObjects.Password.Create(password);

        return new User(
            name: userName,
            email: userEmail,
            passwordHash: passwordHasher.HashPassword(password),
            phoneNumber: userPhone,
            accountType: AccountType.Customer,
            roles: new HashSet<Roles> { Roles.User },
            address: address,
            avatar: avatar
        );
    }

    // Domain Methods
    public bool VerifyPassword(string password, IPasswordHasher passwordHasher)
    {
        if (string.IsNullOrWhiteSpace(password))
            return false;

        return passwordHasher.VerifyPassword(password, PasswordHash);
    }

    public void UpdateProfile(string name, string? address = null, string? avatar = null)
    {
        var userName = ValueObjects.Name.Create(name, minLength: 2, maxLength: 100);

        Name = userName;
        Address = address;
        Avatar = avatar;
        UpdateDate = DateTime.UtcNow;
    }

    public void UpdatePhoneNumber(string phoneNumber)
    {
        var userPhone = Phone.Create(phoneNumber);

        PhoneNumber = userPhone;
        PhoneVerified = false; // Reset verification when phone changes
        UpdateDate = DateTime.UtcNow;
    }

    public void UpdateEmail(string email)
    {
        var userEmail = ValueObjects.Email.Create(email);

        Email = userEmail;
        EmailVerified = false; // Reset verification when email changes
        UpdateDate = DateTime.UtcNow;
    }

    public void UpdateAvatar(string avatarUrl)
    {
        Avatar = avatarUrl;
        UpdateDate = DateTime.UtcNow;
    }

    public void VerifyEmail()
    {
        EmailVerified = true;
        UpdateDate = DateTime.UtcNow;
    }

    public void VerifyPhone()
    {
        PhoneVerified = true;
        UpdateDate = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdateDate = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdateDate = DateTime.UtcNow;
    }

    public void ChangePassword(string currentPassword, string newPassword, IPasswordHasher passwordHasher)
    {
        if (!VerifyPassword(currentPassword, passwordHasher))
        {
            throw new InvalidOperationException("كلمة المرور الحالية غير صحيحة");
        }

        var validPassword = ValueObjects.Password.Create(newPassword);

        PasswordHash = passwordHasher.HashPassword(validPassword);
        UpdateDate = DateTime.UtcNow;
    }

    public void RecordFailedLogin(int maxAttempts = 5, int lockoutMinutes = 15)
    {
        FailedLoginAttempts++;

        if (FailedLoginAttempts >= maxAttempts)
        {
            LockedUntil = DateTime.UtcNow.AddMinutes(lockoutMinutes);
        }

        UpdateDate = DateTime.UtcNow;
    }

    public void ResetFailedLoginAttempts()
    {
        FailedLoginAttempts = 0;
        LockedUntil = null;
        UpdateDate = DateTime.UtcNow;
    }

    public bool IsAccountLocked()
    {
        if (LockedUntil.HasValue && LockedUntil.Value > DateTime.UtcNow)
        {
            return true;
        }

        // Auto-unlock if lock period has expired
        if (LockedUntil.HasValue && LockedUntil.Value <= DateTime.UtcNow)
        {
            LockedUntil = null;
            FailedLoginAttempts = 0;
        }

        return false;
    }


}



