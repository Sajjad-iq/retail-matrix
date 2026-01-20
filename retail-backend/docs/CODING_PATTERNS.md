# Retail Matrix - Coding Patterns Reference

> **CRITICAL FOR AI ASSISTANTS:** This document contains the EXACT code patterns used in this codebase. When implementing new features, you MUST copy these patterns exactly. Do NOT invent new patterns or deviate from these conventions.

---

## Table of Contents

1. [Entity Pattern](#1-entity-pattern)
2. [Value Object Pattern](#2-value-object-pattern)
3. [Repository Pattern](#3-repository-pattern)
4. [Command Pattern (CQRS)](#4-command-pattern-cqrs)
5. [Query Pattern (CQRS)](#5-query-pattern-cqrs)
6. [Validator Pattern](#6-validator-pattern)
7. [DTO Pattern](#7-dto-pattern)
8. [Controller Pattern](#8-controller-pattern)
9. [Service Pattern](#9-service-pattern)
10. [Exception Handling](#10-exception-handling)
11. [AutoMapper Configuration](#11-automapper-configuration)
12. [Naming Conventions](#12-naming-conventions)
13. [Arabic Error Messages](#13-arabic-error-messages)

---

## 1. Entity Pattern

### 1.1 Aggregate Root Template

```csharp
// Location: Domains/{Domain}/Entities/{Entity}.cs
using Domains.Shared.Base;
using Domains.{Domain}.Enums;

namespace Domains.{Domain}.Entities;

/// <summary>
/// {Description} aggregate root
/// </summary>
public class {Entity} : BaseEntity
{
    // ============================================
    // 1. PRIVATE PARAMETERLESS CONSTRUCTOR (EF Core)
    // ============================================
    private {Entity}()
    {
        // Initialize collections and required properties
        Items = new List<{ChildEntity}>();
    }

    // ============================================
    // 2. PRIVATE CONSTRUCTOR (All parameters)
    // ============================================
    private {Entity}(
        Guid organizationId,
        string name,
        {Status} status)
    {
        Id = Guid.NewGuid();
        OrganizationId = organizationId;
        Name = name;
        Status = status;
        Items = new List<{ChildEntity}>();
        InsertDate = DateTime.UtcNow;
    }

    // ============================================
    // 3. PROPERTIES (Private setters)
    // ============================================
    public Guid OrganizationId { get; private set; }
    public string Name { get; private set; }
    public {Status} Status { get; private set; }
    public string? Notes { get; private set; }

    // Navigation properties
    public List<{ChildEntity}> Items { get; private set; }

    // ============================================
    // 4. FACTORY METHOD
    // ============================================
    public static {Entity} Create(
        Guid organizationId,
        string name)
    {
        // Validate using value objects or direct validation
        if (organizationId == Guid.Empty)
            throw new ArgumentException("معرف المؤسسة مطلوب", nameof(organizationId));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("الاسم مطلوب", nameof(name));

        return new {Entity}(
            organizationId: organizationId,
            name: name,
            status: {Status}.Draft
        );
    }

    // ============================================
    // 5. DOMAIN METHODS (State changes)
    // ============================================
    public void Complete()
    {
        if (Status == {Status}.Completed)
            throw new InvalidOperationException("العملية مكتملة بالفعل");

        if (Items.Count == 0)
            throw new InvalidOperationException("لا يمكن إكمال العملية بدون عناصر");

        Status = {Status}.Completed;
    }

    public void Cancel()
    {
        if (Status == {Status}.Completed)
            throw new InvalidOperationException("لا يمكن إلغاء عملية مكتملة");

        Status = {Status}.Cancelled;
    }

    public void AddItem({ChildEntity} item)
    {
        if (Status != {Status}.Draft)
            throw new InvalidOperationException("لا يمكن تعديل عملية غير مسودة");

        Items.Add(item);
    }

    public void UpdateNotes(string? notes)
    {
        Notes = notes;
    }
}
```

### 1.2 Real Example: User Entity

```csharp
// Location: Domains/Users/Entities/User.cs
using Domains.Users.Enums;
using Domains.Users.ValueObjects;
using Domains.Users.Services;
using Domains.Shared.Base;

namespace Domains.Users.Entities;

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
        string? address = null)
    {
        Id = Guid.NewGuid();
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
        PhoneNumber = phoneNumber;
        Address = address;
        AccountType = accountType;
        UserRoles = roles;
        IsActive = true;
        EmailVerified = false;
        PhoneVerified = false;
        FailedLoginAttempts = 0;
        InsertDate = DateTime.UtcNow;
    }

    // Properties with proper encapsulation
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public string PhoneNumber { get; private set; }
    public string? Address { get; private set; }
    public AccountType AccountType { get; private set; }
    public HashSet<Roles> UserRoles { get; private set; }
    public bool IsActive { get; private set; }
    public bool EmailVerified { get; private set; }
    public bool PhoneVerified { get; private set; }
    public int FailedLoginAttempts { get; private set; }
    public DateTime? LockedUntil { get; private set; }

    // Factory method - validates using Value Objects
    public static User CreateBusinessOwner(
        string name,
        string email,
        string password,
        string phoneNumber,
        IPasswordHasher passwordHasher,
        string? address = null)
    {
        // Validate using value objects (throws if invalid)
        var userName = ValueObjects.Name.Create(name, minLength: 2, maxLength: 100);
        var userEmail = ValueObjects.Email.Create(email);
        var userPhone = Phone.Create(phoneNumber);
        var validPassword = ValueObjects.Password.Create(password);

        return new User(
            name: userName,
            email: userEmail,
            passwordHash: passwordHasher.HashPassword(validPassword),
            phoneNumber: userPhone,
            accountType: AccountType.BusinessOwner,
            roles: new HashSet<Roles> { Roles.Owner },
            address: address
        );
    }

    // Domain methods
    public bool VerifyPassword(string password, IPasswordHasher passwordHasher)
    {
        if (string.IsNullOrWhiteSpace(password))
            return false;
        return passwordHasher.VerifyPassword(password, PasswordHash);
    }

    public void RecordFailedLogin(int maxAttempts = 5, int lockoutMinutes = 15)
    {
        FailedLoginAttempts++;
        if (FailedLoginAttempts >= maxAttempts)
        {
            LockedUntil = DateTime.UtcNow.AddMinutes(lockoutMinutes);
        }
    }

    public void ResetFailedLoginAttempts()
    {
        FailedLoginAttempts = 0;
        LockedUntil = null;
    }

    public bool IsAccountLocked()
    {
        return LockedUntil.HasValue && LockedUntil.Value > DateTime.UtcNow;
    }
}
```

### 1.3 Real Example: Sale Entity (Aggregate Root with Children)

```csharp
// Location: Domains/Sales/Entities/Sale.cs
using Domains.Shared.ValueObjects;
using Domains.Sales.Enums;
using Domains.Shared.Base;

namespace Domains.Sales.Entities;

public class Sale : BaseEntity
{
    private const int MaxItemsPerSale = 1000;

    private Sale()
    {
        SaleNumber = string.Empty;
        Items = new List<SaleItem>();
        TotalDiscount = Price.Create(0, "IQD");
        GrandTotal = Price.Create(0, "IQD");
        AmountPaid = Price.Create(0, "IQD");
    }

    private Sale(string saleNumber, Guid organizationId, Guid salesPersonId)
    {
        Id = Guid.NewGuid();
        SaleNumber = saleNumber;
        SaleDate = DateTime.UtcNow;
        OrganizationId = organizationId;
        SalesPersonId = salesPersonId;
        Status = SaleStatus.Draft;
        TotalDiscount = Price.Create(0, "IQD");
        GrandTotal = Price.Create(0, "IQD");
        AmountPaid = Price.Create(0, "IQD");
        Items = new List<SaleItem>();
        InsertDate = DateTime.UtcNow;
    }

    public string SaleNumber { get; private set; }
    public DateTime SaleDate { get; private set; }
    public Guid SalesPersonId { get; private set; }
    public Guid OrganizationId { get; private set; }
    public SaleStatus Status { get; private set; }
    public Price TotalDiscount { get; private set; }
    public Price GrandTotal { get; private set; }
    public Price AmountPaid { get; private set; }
    public List<SaleItem> Items { get; private set; }

    public static Sale Create(Guid organizationId, Guid salesPersonId)
    {
        var saleNumber = GenerateSaleNumber();
        return new Sale(saleNumber, organizationId, salesPersonId);
    }

    // Child entities modified ONLY through aggregate root
    public async Task AddItemAsync(
        Guid productPackagingId,
        string productName,
        int quantity,
        Price unitPrice,
        ICurrencyConversionService currencyService,
        CancellationToken cancellationToken = default)
    {
        if (Status != SaleStatus.Draft)
            throw new InvalidOperationException("لا يمكن تعديل بيع مكتمل");

        if (quantity <= 0)
            throw new ArgumentException("الكمية يجب أن تكون أكبر من صفر", nameof(quantity));

        if (Items.Count >= MaxItemsPerSale)
            throw new InvalidOperationException($"لا يمكن إضافة أكثر من {MaxItemsPerSale} عنصر");

        var item = SaleItem.Create(Id, productPackagingId, productName, quantity, unitPrice);
        Items.Add(item);
        await RecalculateTotalsAsync(currencyService, cancellationToken);
    }

    public void CompleteSale()
    {
        if (Status == SaleStatus.Completed)
            throw new InvalidOperationException("البيع مكتمل بالفعل");

        if (Items.Count == 0)
            throw new InvalidOperationException("لا يمكن إكمال بيع بدون عناصر");

        if (AmountPaid.Amount < GrandTotal.Amount)
            throw new InvalidOperationException("المبلغ المدفوع أقل من الإجمالي");

        Status = SaleStatus.Completed;
    }

    private static string GenerateSaleNumber()
    {
        var date = DateTime.UtcNow.ToString("yyyyMMdd");
        var uniqueId = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
        return $"SAL-{date}-{uniqueId}";
    }
}
```

---

## 2. Value Object Pattern

### 2.1 Value Object Template

```csharp
// Location: Domains/{Domain}/ValueObjects/{ValueObject}.cs
namespace Domains.{Domain}.ValueObjects;

/// <summary>
/// Value object representing {description}
/// </summary>
public sealed class {ValueObject} : IEquatable<{ValueObject}>
{
    public {Type} Value { get; }

    // Private constructor
    private {ValueObject}({Type} value)
    {
        Value = value;
    }

    // Factory method with validation
    public static {ValueObject} Create({Type} value)
    {
        if (/* validation fails */)
            throw new ArgumentException("رسالة خطأ بالعربية", nameof(value));

        return new {ValueObject}(value);
    }

    // Equality members
    public bool Equals({ValueObject}? other)
    {
        if (other is null) return false;
        return Value.Equals(other.Value);
    }

    public override bool Equals(object? obj) => Equals(obj as {ValueObject});
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value.ToString();

    // Implicit conversion to underlying type
    public static implicit operator {Type}({ValueObject} vo) => vo.Value;
}
```

### 2.2 Real Example: Email Value Object

```csharp
// Location: Domains/Users/ValueObjects/Email.cs
namespace Domains.Users.ValueObjects;

public sealed class Email : IEquatable<Email>
{
    public string Value { get; }

    private Email(string value)
    {
        Value = value;
    }

    public static Email Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("البريد الإلكتروني مطلوب", nameof(email));

        var normalizedEmail = email.Trim().ToLowerInvariant();

        if (!IsValidEmail(normalizedEmail))
            throw new ArgumentException("صيغة البريد الإلكتروني غير صحيحة", nameof(email));

        if (normalizedEmail.Length > 254)
            throw new ArgumentException("البريد الإلكتروني يجب ألا يتجاوز 254 حرف", nameof(email));

        return new Email(normalizedEmail);
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch (FormatException)
        {
            return false;
        }
    }

    public bool Equals(Email? other)
    {
        if (other is null) return false;
        return Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object? obj) => Equals(obj as Email);
    public override int GetHashCode() => Value.GetHashCode(StringComparison.OrdinalIgnoreCase);
    public override string ToString() => Value;
    public static implicit operator string(Email email) => email.Value;
}
```

### 2.3 Real Example: Price Value Object (Composite)

```csharp
// Location: Domains/Shared/ValueObjects/Price.cs
namespace Domains.Shared.ValueObjects;

public sealed class Price : IEquatable<Price>
{
    public decimal Amount { get; }
    public string Currency { get; }

    // Parameterless constructor for EF Core
    private Price()
    {
        Amount = 0;
        Currency = "IQD";
    }

    private Price(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public static Price Create(decimal amount, string currency = "IQD")
    {
        if (amount < 0)
            throw new ArgumentException("السعر لا يمكن أن يكون سالب", nameof(amount));

        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("العملة مطلوبة", nameof(currency));

        var normalizedCurrency = currency.Trim().ToUpperInvariant();

        if (normalizedCurrency.Length != 3)
            throw new ArgumentException("رمز العملة يجب أن يكون 3 أحرف (مثل IQD, USD)", nameof(currency));

        return new Price(amount, normalizedCurrency);
    }

    // Domain operations
    public Price Add(Price other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException($"لا يمكن جمع أسعار بعملات مختلفة: {Currency} و {other.Currency}");

        return new Price(Amount + other.Amount, Currency);
    }

    public Price Subtract(Price other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException($"لا يمكن طرح أسعار بعملات مختلفة");

        var result = Amount - other.Amount;
        if (result < 0)
            throw new InvalidOperationException("النتيجة لا يمكن أن تكون سالبة");

        return new Price(result, Currency);
    }

    public Price Multiply(decimal factor)
    {
        if (factor < 0)
            throw new ArgumentException("المضاعف لا يمكن أن يكون سالب", nameof(factor));

        return new Price(Amount * factor, Currency);
    }

    public bool Equals(Price? other)
    {
        if (other is null) return false;
        return Amount == other.Amount && Currency.Equals(other.Currency, StringComparison.Ordinal);
    }

    public override bool Equals(object? obj) => Equals(obj as Price);
    public override int GetHashCode() => HashCode.Combine(Amount, Currency);
    public override string ToString() => $"{Amount:N2} {Currency}";
}
```

---

## 3. Repository Pattern

### 3.1 Repository Interface Template

```csharp
// Location: Domains/{Domain}/Repositories/I{Entity}Repository.cs
using Domains.{Domain}.Entities;
using Domains.Shared.Base;

namespace Domains.{Domain}.Repositories;

public interface I{Entity}Repository : IRepository<{Entity}>
{
    // Domain-specific query methods
    Task<{Entity}?> GetBy{UniqueField}Async(string value, CancellationToken ct = default);
    Task<List<{Entity}>> GetByOrganizationAsync(Guid organizationId, CancellationToken ct = default);
    Task<bool> ExistsBy{UniqueField}Async(string value, CancellationToken ct = default);
}
```

### 3.2 Repository Implementation Template

```csharp
// Location: Infrastructure/Repositories/{Entity}Repository.cs
using Domains.{Domain}.Entities;
using Domains.{Domain}.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class {Entity}Repository : Repository<{Entity}>, I{Entity}Repository
{
    public {Entity}Repository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<{Entity}?> GetBy{UniqueField}Async(string value, CancellationToken ct = default)
    {
        return await _dbSet
            .AsNoTracking()  // ALWAYS use for read operations
            .FirstOrDefaultAsync(e => e.{Field} == value, ct);
    }

    public async Task<List<{Entity}>> GetByOrganizationAsync(Guid organizationId, CancellationToken ct = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(e => e.OrganizationId == organizationId)
            .ToListAsync(ct);
    }

    public async Task<bool> ExistsBy{UniqueField}Async(string value, CancellationToken ct = default)
    {
        return await _dbSet
            .AsNoTracking()
            .AnyAsync(e => e.{Field} == value, ct);
    }
}
```

### 3.3 Real Example: UserRepository

```csharp
// Location: Infrastructure/Repositories/UserRepository.cs
using Domains.Users.Entities;
using Domains.Users.Repositories;
using Domains.Users.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant(), ct);
    }

    public async Task<User?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken ct = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber, ct);
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default)
    {
        return await _dbSet
            .AsNoTracking()
            .AnyAsync(u => u.Email == email.ToLowerInvariant(), ct);
    }

    public async Task<bool> ExistsByPhoneNumberAsync(string phoneNumber, CancellationToken ct = default)
    {
        return await _dbSet
            .AsNoTracking()
            .AnyAsync(u => u.PhoneNumber == phoneNumber, ct);
    }
}
```

---

## 4. Command Pattern (CQRS)

### 4.1 Command Template

```csharp
// Location: Application/{Domain}/Commands/{Action}/{Action}Command.cs
using MediatR;

namespace Application.{Domain}.Commands.{Action};

public record {Action}Command : IRequest<{ReturnType}>
{
    public {Type} {Property} { get; init; }
    public {Type} {Property2} { get; init; }
    // Add all required properties with { get; init; }
}
```

### 4.2 Command Handler Template

```csharp
// Location: Application/{Domain}/Commands/{Action}/{Action}CommandHandler.cs
using Application.Common.Exceptions;
using Domains.{Domain}.Entities;
using Domains.{Domain}.Repositories;
using MediatR;

namespace Application.{Domain}.Commands.{Action};

public class {Action}CommandHandler : IRequestHandler<{Action}Command, {ReturnType}>
{
    private readonly I{Entity}Repository _{entity}Repository;
    // Inject other dependencies as needed

    public {Action}CommandHandler(I{Entity}Repository {entity}Repository)
    {
        _{entity}Repository = {entity}Repository;
    }

    public async Task<{ReturnType}> Handle({Action}Command request, CancellationToken cancellationToken)
    {
        // 1. Validate business rules (duplicates, existence, etc.)
        // 2. Create/modify entity using factory method or domain methods
        // 3. Persist changes
        // 4. Return result
    }
}
```

### 4.3 Real Example: RegisterCommand

```csharp
// Location: Application/Auth/Commands/Register/RegisterCommand.cs
using MediatR;

namespace Application.Auth.Commands.Register;

public record RegisterCommand : IRequest<Guid>
{
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public string? Address { get; init; }
}
```

```csharp
// Location: Application/Auth/Commands/Register/RegisterCommandHandler.cs
using Application.Common.Exceptions;
using Domains.Users.Entities;
using Domains.Users.Repositories;
using Domains.Users.Services;
using MediatR;

namespace Application.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Guid>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<Guid> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        // 1. Check if email already exists
        if (await _userRepository.ExistsByEmailAsync(request.Email, cancellationToken))
        {
            throw new ValidationException("البريد الإلكتروني مستخدم بالفعل");
        }

        // 2. Check if phone number already exists
        if (await _userRepository.ExistsByPhoneNumberAsync(request.PhoneNumber, cancellationToken))
        {
            throw new ValidationException("رقم الهاتف مستخدم بالفعل");
        }

        // 3. Create entity using factory method
        var user = User.CreateBusinessOwner(
            name: request.Name,
            email: request.Email,
            password: request.Password,
            phoneNumber: request.PhoneNumber,
            passwordHasher: _passwordHasher,
            address: request.Address
        );

        // 4. Persist user
        await _userRepository.AddAsync(user, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);

        // 5. Return user ID
        return user.Id;
    }
}
```

### 4.4 Real Example: LoginCommand (Returns DTO)

```csharp
// Location: Application/Auth/Commands/Login/LoginCommand.cs
using Application.Auth.DTOs;
using MediatR;

namespace Application.Auth.Commands.Login;

public record LoginCommand : IRequest<TokenDto>
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}
```

```csharp
// Location: Application/Auth/Commands/Login/LoginCommandHandler.cs
using Application.Auth.DTOs;
using Application.Common.Exceptions;
using Application.Common.Services;
using AutoMapper;
using Domains.Users.Repositories;
using Domains.Users.Services;
using MediatR;

namespace Application.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, TokenDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IMapper _mapper;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _mapper = mapper;
    }

    public async Task<TokenDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // 1. Retrieve user by email
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user == null)
            throw new UnauthorizedException("البريد الإلكتروني أو كلمة المرور غير صحيحة");

        // 2. Check if account is locked
        if (user.IsAccountLocked())
            throw new UnauthorizedException("الحساب مقفل مؤقتاً بسبب محاولات تسجيل دخول فاشلة متعددة");

        // 3. Verify password
        if (!user.VerifyPassword(request.Password, _passwordHasher))
        {
            user.RecordFailedLogin();
            await _userRepository.UpdateAsync(user, cancellationToken);
            await _userRepository.SaveChangesAsync(cancellationToken);
            throw new UnauthorizedException("البريد الإلكتروني أو كلمة المرور غير صحيحة");
        }

        // 4. Check if account is active
        if (!user.IsActive)
            throw new UnauthorizedException("الحساب غير نشط");

        // 5. Reset failed login attempts
        user.ResetFailedLoginAttempts();
        await _userRepository.UpdateAsync(user, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);

        // 6. Generate tokens
        var accessToken = _jwtTokenService.GenerateAccessToken(user);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();
        var expiresAt = _jwtTokenService.GetTokenExpiration();

        // 7. Map and return
        var userDto = _mapper.Map<UserDto>(user);

        return new TokenDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = expiresAt,
            User = userDto
        };
    }
}
```

---

## 5. Query Pattern (CQRS)

### 5.1 Query Template

```csharp
// Location: Application/{Domain}/Queries/{Query}/{Query}Query.cs
using Application.{Domain}.DTOs;
using MediatR;

namespace Application.{Domain}.Queries.{Query};

public record {Query}Query({Parameters}) : IRequest<{ReturnDto}?>;
```

### 5.2 Query Handler Template

```csharp
// Location: Application/{Domain}/Queries/{Query}/{Query}QueryHandler.cs
using Application.Common.Exceptions;
using Application.{Domain}.DTOs;
using AutoMapper;
using Domains.{Domain}.Repositories;
using MediatR;

namespace Application.{Domain}.Queries.{Query};

public class {Query}QueryHandler : IRequestHandler<{Query}Query, {ReturnDto}?>
{
    private readonly I{Entity}Repository _{entity}Repository;
    private readonly IMapper _mapper;

    public {Query}QueryHandler(I{Entity}Repository {entity}Repository, IMapper mapper)
    {
        _{entity}Repository = {entity}Repository;
        _mapper = mapper;
    }

    public async Task<{ReturnDto}?> Handle({Query}Query request, CancellationToken cancellationToken)
    {
        var entity = await _{entity}Repository.GetByIdAsync(request.Id, cancellationToken);

        if (entity == null)
            throw new NotFoundException($"{Entity} with ID {request.Id} not found");

        return _mapper.Map<{ReturnDto}>(entity);
    }
}
```

### 5.3 Real Example: GetCurrentUserQuery

```csharp
// Location: Application/Auth/Queries/GetCurrentUser/GetCurrentUserQuery.cs
using Application.Auth.DTOs;
using MediatR;

namespace Application.Auth.Queries.GetCurrentUser;

public record GetCurrentUserQuery(Guid UserId) : IRequest<UserDto?>;
```

```csharp
// Location: Application/Auth/Queries/GetCurrentUser/GetCurrentUserQueryHandler.cs
using Application.Auth.DTOs;
using AutoMapper;
using Domains.Users.Repositories;
using MediatR;

namespace Application.Auth.Queries.GetCurrentUser;

public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, UserDto?>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetCurrentUserQueryHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<UserDto?> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user == null)
            return null;

        return _mapper.Map<UserDto>(user);
    }
}
```

---

## 6. Validator Pattern

### 6.1 Validator Template

```csharp
// Location: Application/{Domain}/Commands/{Action}/{Action}CommandValidator.cs
using FluentValidation;

namespace Application.{Domain}.Commands.{Action};

public class {Action}CommandValidator : AbstractValidator<{Action}Command>
{
    public {Action}CommandValidator()
    {
        // ONLY validate required fields and format
        // Business rules are validated in Handler or Domain

        RuleFor(x => x.{Property})
            .NotEmpty()
            .WithMessage("{PropertyArabicName} مطلوب");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("البريد الإلكتروني مطلوب")
            .EmailAddress()
            .WithMessage("صيغة البريد الإلكتروني غير صحيحة");

        RuleFor(x => x.{NumericProperty})
            .GreaterThan(0)
            .WithMessage("{PropertyArabicName} يجب أن يكون أكبر من صفر");
    }
}
```

### 6.2 Real Example: RegisterCommandValidator

```csharp
// Location: Application/Auth/Commands/Register/RegisterCommandValidator.cs
using FluentValidation;

namespace Application.Auth.Commands.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("الاسم مطلوب");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("البريد الإلكتروني مطلوب");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("كلمة المرور مطلوبة");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .WithMessage("رقم الهاتف مطلوب");
    }
}
```

### 6.3 Real Example: LoginCommandValidator

```csharp
// Location: Application/Auth/Commands/Login/LoginCommandValidator.cs
using FluentValidation;

namespace Application.Auth.Commands.Login;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("البريد الإلكتروني مطلوب");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("كلمة المرور مطلوبة");
    }
}
```

---

## 7. DTO Pattern

### 7.1 DTO Template

```csharp
// Location: Application/{Domain}/DTOs/{Entity}Dto.cs
namespace Application.{Domain}.DTOs;

public record {Entity}Dto
{
    public Guid Id { get; init; }
    public string {Property} { get; init; } = string.Empty;
    public decimal {NumericProperty} { get; init; }
    public string Status { get; init; } = string.Empty;  // Enums as strings
    public DateTime CreatedAt { get; init; }
    public List<{ChildEntity}Dto> Items { get; init; } = new();
}

public record {ChildEntity}Dto
{
    public Guid Id { get; init; }
    public string {Property} { get; init; } = string.Empty;
}
```

### 7.2 Real Example: UserDto

```csharp
// Location: Application/Auth/DTOs/UserDto.cs
namespace Application.Auth.DTOs;

public record UserDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public string? Address { get; init; }
    public string AccountType { get; init; } = string.Empty;  // Enum as string
    public List<string> Roles { get; init; } = new();         // Enums as strings
    public bool IsActive { get; init; }
    public bool EmailVerified { get; init; }
    public bool PhoneVerified { get; init; }
}
```

### 7.3 Real Example: TokenDto

```csharp
// Location: Application/Auth/DTOs/TokenDto.cs
namespace Application.Auth.DTOs;

public record TokenDto
{
    public string AccessToken { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
    public DateTime ExpiresAt { get; init; }
    public UserDto User { get; init; } = null!;
}
```

---

## 8. Controller Pattern

### 8.1 Controller Template

```csharp
// Location: API/Controllers/{Domain}Controller.cs
using Application.{Domain}.Commands.{Action};
using Application.{Domain}.Queries.{Query};
using Application.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class {Domain}Controller : ControllerBase
{
    private readonly IMediator _mediator;

    public {Domain}Controller(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// {Description}
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<Guid>>> Create([FromBody] Create{Entity}Command command)
    {
        var id = await _mediator.Send(command);
        var response = ApiResponse<Guid>.SuccessResponse(id, "تم إنشاء {EntityArabic} بنجاح");
        return CreatedAtAction(nameof(GetById), new { id }, response);
    }

    /// <summary>
    /// {Description}
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<{Entity}Dto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<{Entity}Dto>>> GetById(Guid id)
    {
        var result = await _mediator.Send(new Get{Entity}ByIdQuery(id));
        var response = ApiResponse<{Entity}Dto>.SuccessResponse(result, "تم جلب البيانات بنجاح");
        return Ok(response);
    }

    /// <summary>
    /// {Description} - Protected endpoint
    /// </summary>
    [HttpGet("protected")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<object>>> ProtectedAction()
    {
        // Extract user ID from claims
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(ApiErrorResponse.Create("غير مصرح لك بالوصول"));
        }

        // Process...
    }
}
```

### 8.2 Real Example: AuthController

```csharp
// Location: API/Controllers/AuthController.cs
using Application.Auth.Commands.Login;
using Application.Auth.Commands.Register;
using Application.Auth.Queries.GetCurrentUser;
using Application.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<Guid>>> Register([FromBody] RegisterCommand command)
    {
        var userId = await _mediator.Send(command);
        var response = ApiResponse<Guid>.SuccessResponse(userId, "تم تسجيل المستخدم بنجاح");
        return CreatedAtAction(nameof(GetCurrentUser), new { id = userId }, response);
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<object>>> Login([FromBody] LoginCommand command)
    {
        var token = await _mediator.Send(command);
        var response = ApiResponse<object>.SuccessResponse(token, "تم تسجيل الدخول بنجاح");
        return Ok(response);
    }

    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<object>>> GetCurrentUser()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(ApiErrorResponse.Create("غير مصرح لك بالوصول"));
        }

        var user = await _mediator.Send(new GetCurrentUserQuery(userId));

        if (user == null)
        {
            return NotFound(ApiErrorResponse.Create("المستخدم غير موجود"));
        }

        var response = ApiResponse<object>.SuccessResponse(user, "تم جلب بيانات المستخدم بنجاح");
        return Ok(response);
    }
}
```

---

## 9. Service Pattern

### 9.1 Service Interface Template (Domain Layer)

```csharp
// Location: Domains/{Domain}/Services/I{Service}.cs
namespace Domains.{Domain}.Services;

public interface I{Service}
{
    {ReturnType} {Method}({Parameters});
}
```

### 9.2 Service Interface Template (Application Layer)

```csharp
// Location: Application/Common/Services/I{Service}.cs
namespace Application.Common.Services;

public interface I{Service}
{
    {ReturnType} {Method}({Parameters});
}
```

### 9.3 Service Implementation Template

```csharp
// Location: Infrastructure/Services/{Service}.cs
using Application.Common.Services;  // or Domains.{Domain}.Services

namespace Infrastructure.Services;

public class {Service} : I{Service}
{
    private readonly {Dependencies};

    public {Service}({Dependencies})
    {
        // Initialize dependencies
    }

    public {ReturnType} {Method}({Parameters})
    {
        // Implementation
    }
}
```

---

## 10. Exception Handling

### 10.1 Custom Exceptions

```csharp
// Location: Application/Common/Exceptions/ValidationException.cs
using FluentValidation.Results;

namespace Application.Common.Exceptions;

public class ValidationException : Exception
{
    public List<string> Errors { get; }

    public ValidationException() : base("حدثت أخطاء في التحقق من الصحة")
    {
        Errors = new List<string>();
    }

    public ValidationException(IEnumerable<ValidationFailure> failures) : this()
    {
        Errors = failures.Select(f => f.ErrorMessage).ToList();
    }

    public ValidationException(string error) : this()
    {
        Errors = new List<string> { error };
    }
}
```

```csharp
// Location: Application/Common/Exceptions/NotFoundException.cs
namespace Application.Common.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}
```

```csharp
// Location: Application/Common/Exceptions/UnauthorizedException.cs
namespace Application.Common.Exceptions;

public class UnauthorizedException : Exception
{
    public UnauthorizedException(string message) : base(message) { }
}
```

### 10.2 Global Exception Filter

```csharp
// Location: API/Filters/GlobalExceptionFilter.cs
using Application.Common.Exceptions;
using Application.Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Filters;

public class GlobalExceptionFilter : IExceptionFilter
{
    private readonly ILogger<GlobalExceptionFilter> _logger;

    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
    {
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        var exception = context.Exception;
        _logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);

        ApiErrorResponse errorResponse;
        int statusCode;

        switch (exception)
        {
            case ValidationException validationEx:
                statusCode = StatusCodes.Status400BadRequest;
                errorResponse = ApiErrorResponse.Create("فشل التحقق من الصحة", validationEx.Errors);
                break;

            case UnauthorizedException unauthorizedEx:
                statusCode = StatusCodes.Status401Unauthorized;
                errorResponse = ApiErrorResponse.Create(unauthorizedEx.Message);
                break;

            case NotFoundException notFoundEx:
                statusCode = StatusCodes.Status404NotFound;
                errorResponse = ApiErrorResponse.Create(notFoundEx.Message);
                break;

            default:
                statusCode = StatusCodes.Status500InternalServerError;
                errorResponse = ApiErrorResponse.Create("حدث خطأ في الخادم. يرجى المحاولة مرة أخرى لاحقاً");
                break;
        }

        context.Result = new ObjectResult(errorResponse) { StatusCode = statusCode };
        context.ExceptionHandled = true;
    }
}
```

---

## 11. AutoMapper Configuration

### 11.1 Mapping Profile

```csharp
// Location: Application/Common/Mappings/MappingProfile.cs
using Application.Auth.DTOs;
using AutoMapper;
using Domains.Users.Entities;

namespace Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User mappings
        CreateMap<User, UserDto>()
            .ForMember(d => d.AccountType, opt => opt.MapFrom(s => s.AccountType.ToString()))
            .ForMember(d => d.Roles, opt => opt.MapFrom(s => s.UserRoles.Select(r => r.ToString()).ToList()));

        // Add more mappings as needed
        // CreateMap<Entity, EntityDto>();
    }
}
```

### 11.2 Mapping Rules

1. **Enums** → Map to `string` using `.ToString()`
2. **Value Objects** → Map to primitive types (Amount, Currency separately)
3. **Collections** → Map to List of DTOs
4. **Navigation Properties** → Exclude or map to nested DTOs

---

## 12. Naming Conventions

| Type | Pattern | Example |
|------|---------|---------|
| Entity | `{Name}` | `User`, `Sale`, `Product` |
| Value Object | `{Concept}` | `Price`, `Email`, `Phone` |
| Enum | `{Name}` | `SaleStatus`, `Roles`, `AccountType` |
| Repository Interface | `I{Entity}Repository` | `IUserRepository` |
| Repository Impl | `{Entity}Repository` | `UserRepository` |
| Command | `{Verb}{Entity}Command` | `RegisterCommand`, `CreateSaleCommand` |
| Query | `Get{Entity}By{Criteria}Query` | `GetCurrentUserQuery`, `GetSaleByIdQuery` |
| Handler | `{Request}Handler` | `RegisterCommandHandler` |
| Validator | `{Request}Validator` | `RegisterCommandValidator` |
| DTO | `{Entity}Dto` | `UserDto`, `SaleDto` |
| Service Interface | `I{Name}Service` | `IJwtTokenService` |
| Controller | `{Domain}Controller` | `AuthController`, `SalesController` |
| Folder (Command) | `{Verb}{Entity}` | `Register`, `CreateSale`, `AddSaleItem` |
| Folder (Query) | `Get{Entity}By{Criteria}` | `GetCurrentUser`, `GetSaleById` |

---

## 13. Arabic Error Messages

### 13.1 Common Messages

| Context | Arabic | English (for reference) |
|---------|--------|------------------------|
| Required field | `{الحقل} مطلوب` | {Field} is required |
| Invalid format | `صيغة {الحقل} غير صحيحة` | {Field} format is invalid |
| Already exists | `{الحقل} مستخدم بالفعل` | {Field} already exists |
| Not found | `{الكيان} غير موجود` | {Entity} not found |
| Unauthorized | `غير مصرح لك بالوصول` | Unauthorized access |
| Account locked | `الحساب مقفل مؤقتاً` | Account temporarily locked |
| Invalid credentials | `البريد الإلكتروني أو كلمة المرور غير صحيحة` | Invalid email or password |
| Operation success | `تم {العملية} بنجاح` | {Operation} successful |
| Server error | `حدث خطأ في الخادم` | Server error occurred |
| Validation failed | `فشل التحقق من الصحة` | Validation failed |

### 13.2 Field-Specific Messages

| Field | Arabic |
|-------|--------|
| Name | الاسم |
| Email | البريد الإلكتروني |
| Password | كلمة المرور |
| Phone | رقم الهاتف |
| Organization | المؤسسة |
| Product | المنتج |
| Sale | البيع |
| Quantity | الكمية |
| Price | السعر |
| Amount | المبلغ |
| Currency | العملة |

---

**REMEMBER:** Always follow these patterns exactly. Do not invent new patterns or deviate from the established conventions.
