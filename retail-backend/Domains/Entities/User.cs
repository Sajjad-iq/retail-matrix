using Domains.Shared;

namespace Domains.Entities;

public class User : BaseEntity
{
    // Private constructor to enforce factory methods
    private User() { }
    // Properties with proper encapsulation
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string PhoneNumber { get; private set; } = string.Empty;
    public string? Address { get; private set; }
    public string? Avatar { get; private set; }
    public List<Role> UserRoles { get; private set; } = new();

    // Status & Verification
    public bool IsActive { get; private set; } = true;
    public bool EmailVerified { get; private set; } = false;
    public bool PhoneVerified { get; private set; } = false;

    // Security - Account Locking
    public int FailedLoginAttempts { get; private set; } = 0;
    public DateTime? LockedUntil { get; private set; }

    // Business Context
    public string? Department { get; private set; }
    public string? MemberOfOrganization { get; private set; }

    public static User CreateBusinessOwner(User user)
    {
        ValidateUserForCreation(user);

        var newUser = new User
        {
            Name = user.Name,
            Email = user.Email,
            PasswordHash = user.PasswordHash,
            PhoneNumber = user.PhoneNumber,
            Address = user.Address,
            Avatar = user.Avatar,
            UserRoles = [Role.BusinessOwner]
        };

        return newUser;
    }

    public static User CreateEmployee(User user)
    {
        if (user.MemberOfOrganization == null)
            throw new Exception("معرف المؤسسة مطلوب للموظف");

        ValidateUserForCreation(user);

        var newUser = new User
        {
            Name = user.Name,
            Email = user.Email,
            PasswordHash = user.PasswordHash,
            PhoneNumber = user.PhoneNumber,
            Address = user.Address,
            Avatar = user.Avatar,
            UserRoles = [Role.Employee],
            MemberOfOrganization = user.MemberOfOrganization,
            Department = user.Department
        };

        return newUser;
    }

    public static User CreateCustomer(User user)
    {
        ValidateUserForCreation(user);

        var newUser = new User
        {
            Name = user.Name,
            Email = user.Email,
            PasswordHash = user.PasswordHash,
            PhoneNumber = user.PhoneNumber,
            Address = user.Address,
            Avatar = user.Avatar,
            UserRoles = [Role.Customer]
        };

        return newUser;
    }



    private static void ValidateUserForCreation(User user)
    {
        if (!user.UserRoles.Contains(Role.Customer) && user.Email == String.Empty) throw new Exception("البريد الإلكتروني مطلوب");

        if (user.Name == String.Empty || user.PasswordHash == String.Empty)
            throw new Exception("الاسم وكلمة المرور مطلوبان");
    }

}


public enum Role
{
    Admin,
    BusinessOwner,
    Customer,
    Employee
}

