using Domains.Shared;
using Domains.Services;

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
        UserRoles = new List<Roles>();
    }

    // Private constructor to enforce factory methods
    private User(
        string name,
        string email,
        string passwordHash,
        string phoneNumber,
        List<Roles> roles,
        string? address = null,
        string? avatar = null,
        string? memberOfOrganization = null,
        string? department = null)
    {
        Id = Guid.NewGuid();
        Name = name;
        Email = email.ToLowerInvariant();
        PasswordHash = passwordHash;
        PhoneNumber = phoneNumber;
        Address = address;
        Avatar = avatar;
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
    public List<Roles> UserRoles { get; private set; }

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
        string? address = null,
        string? avatar = null)
    {
        ValidateUserForCreation(name, email, password, Roles.BusinessOwner);

        return new User(
            name: name,
            email: email,
            passwordHash: passwordHasher.HashPassword(password),
            phoneNumber: phoneNumber,
            roles: [Roles.BusinessOwner],
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
        string? department = null,
        string? address = null,
        string? avatar = null)
    {
        if (string.IsNullOrWhiteSpace(memberOfOrganization))
            throw new ArgumentException("معرف المؤسسة مطلوب للموظف", nameof(memberOfOrganization));

        ValidateUserForCreation(name, email, password, Roles.Employee);

        return new User(
            name: name,
            email: email,
            passwordHash: passwordHasher.HashPassword(password),
            phoneNumber: phoneNumber,
            roles: [Roles.Employee],
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
        ValidateUserForCreation(name, email, password, Roles.Customer);

        return new User(
            name: name,
            email: email,
            passwordHash: passwordHasher.HashPassword(password),
            phoneNumber: phoneNumber,
            roles: [Roles.Customer],
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
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("الاسم مطلوب", nameof(name));

        if (name.Length < 2 || name.Length > 100)
            throw new ArgumentException("الاسم يجب أن يكون بين 2 و 100 حرف", nameof(name));

        Name = name;
        Address = address;
        Avatar = avatar;
        UpdateDate = DateTime.UtcNow;
    }

    public void UpdatePhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("رقم الهاتف مطلوب", nameof(phoneNumber));

        PhoneNumber = phoneNumber;
        PhoneVerified = false; // Reset verification when phone changes
        UpdateDate = DateTime.UtcNow;
    }

    public void UpdateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("البريد الإلكتروني مطلوب", nameof(email));

        if (!IsValidEmail(email))
            throw new ArgumentException("صيغة البريد الإلكتروني غير صحيحة", nameof(email));

        Email = email.ToLowerInvariant();
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

        if (string.IsNullOrWhiteSpace(newPassword))
        {
            throw new ArgumentException("كلمة المرور الجديدة مطلوبة", nameof(newPassword));
        }

        if (newPassword.Length < 8)
        {
            throw new ArgumentException("كلمة المرور يجب أن تكون 8 أحرف على الأقل", nameof(newPassword));
        }

        PasswordHash = passwordHasher.HashPassword(newPassword);
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

    // Private Validation Methods
    private static void ValidateUserForCreation(string name, string email, string password, Roles role)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("الاسم مطلوب", nameof(name));

        if (name.Length < 2 || name.Length > 100)
            throw new ArgumentException("الاسم يجب أن يكون بين 2 و 100 حرف", nameof(name));

        if (role != Roles.Customer && string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("البريد الإلكتروني مطلوب", nameof(email));

        if (!string.IsNullOrWhiteSpace(email) && !IsValidEmail(email))
            throw new ArgumentException("صيغة البريد الإلكتروني غير صحيحة", nameof(email));

        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("كلمة المرور مطلوبة", nameof(password));

        if (password.Length < 8)
            throw new ArgumentException("كلمة المرور يجب أن تكون 8 أحرف على الأقل", nameof(password));

    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

}



