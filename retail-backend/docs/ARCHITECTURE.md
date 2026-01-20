# Retail Matrix - Architecture Reference

> **IMPORTANT FOR AI ASSISTANTS:** This document defines the EXACT architecture and patterns used in this codebase. When implementing new features, you MUST follow these patterns precisely. Do NOT deviate from these conventions.

---

## Table of Contents

1. [Project Overview](#1-project-overview)
2. [Layer Architecture](#2-layer-architecture)
3. [Folder Structure](#3-folder-structure)
4. [Domain Layer](#4-domain-layer)
5. [Application Layer](#5-application-layer)
6. [Infrastructure Layer](#6-infrastructure-layer)
7. [API Layer](#7-api-layer)
8. [Dependency Injection](#8-dependency-injection)
9. [Database & EF Core](#9-database--ef-core)

---

## 1. Project Overview

**Retail Matrix** is a multi-tenant retail management system built with:

| Technology | Version | Purpose |
|------------|---------|---------|
| .NET | 9.0 | Framework |
| ASP.NET Core | 9.0 | Web API |
| Entity Framework Core | 9.0 | ORM |
| MySQL (Pomelo) | 9.0 | Database |
| MediatR | 11.1.0 | CQRS pattern |
| FluentValidation | 12.1.1 | Input validation |
| AutoMapper | 12.0.1 | Object mapping |
| BCrypt.Net | 4.0.3 | Password hashing |
| JWT Bearer | 9.0.1 | Authentication |

### Business Domains

| Domain | Purpose | Aggregate Root |
|--------|---------|----------------|
| Users | Authentication, user management | `User` |
| Organizations | Multi-tenant support | `Organization` |
| Products | Product catalog with packaging variants | `Product` |
| Sales | Point of sale transactions | `Sale` |
| Installments | Payment plans and schedules | `InstallmentPlan` |
| Stocks | Stock levels and batch tracking | `Stock` |
| Inventory | Warehouse operations | `InventoryOperation` |
| Currency | Multi-currency support | `Currency` |

---

## 2. Layer Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                        API Layer                            │
│         Controllers, Filters, Configuration                 │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                    Application Layer                        │
│       Commands, Queries, Handlers, DTOs, Validators         │
└─────────────────────────────────────────────────────────────┘
                              │
              ┌───────────────┴───────────────┐
              ▼                               ▼
┌─────────────────────────────┐   ┌─────────────────────────────┐
│       Domain Layer          │   │    Infrastructure Layer     │
│  Entities, Value Objects,   │   │   Repositories, Services,   │
│  Enums, Repository Interfaces│   │   DbContext, External APIs  │
└─────────────────────────────┘   └─────────────────────────────┘
```

### Dependency Rules (CRITICAL)

```
API → Application → Domain
           ↓
    Infrastructure → Domain
```

- **Domain Layer**: NO dependencies (pure business logic)
- **Application Layer**: Depends on Domain only
- **Infrastructure Layer**: Depends on Domain only (implements interfaces)
- **API Layer**: Depends on Application only

---

## 3. Folder Structure

```
retail-backend/
├── API/                                    # Presentation Layer
│   ├── Configuration/
│   │   └── AuthenticationConfiguration.cs  # JWT setup
│   ├── Controllers/
│   │   └── AuthController.cs               # REST endpoints
│   └── Filters/
│       └── GlobalExceptionFilter.cs        # Exception handling
│
├── Application/                            # Application Layer
│   ├── Auth/                               # Feature folder
│   │   ├── Commands/
│   │   │   ├── Login/
│   │   │   │   ├── LoginCommand.cs
│   │   │   │   ├── LoginCommandHandler.cs
│   │   │   │   └── LoginCommandValidator.cs
│   │   │   └── Register/
│   │   │       ├── RegisterCommand.cs
│   │   │       ├── RegisterCommandHandler.cs
│   │   │       └── RegisterCommandValidator.cs
│   │   ├── Queries/
│   │   │   └── GetCurrentUser/
│   │   │       ├── GetCurrentUserQuery.cs
│   │   │       └── GetCurrentUserQueryHandler.cs
│   │   └── DTOs/
│   │       ├── UserDto.cs
│   │       └── TokenDto.cs
│   ├── Common/
│   │   ├── Behaviors/
│   │   │   └── ValidationBehavior.cs       # MediatR pipeline
│   │   ├── Exceptions/
│   │   │   ├── ValidationException.cs
│   │   │   ├── NotFoundException.cs
│   │   │   └── UnauthorizedException.cs
│   │   ├── Mappings/
│   │   │   └── MappingProfile.cs           # AutoMapper config
│   │   ├── Models/
│   │   │   └── ApiResponse.cs              # Response wrappers
│   │   └── Services/
│   │       └── IJwtTokenService.cs         # Service interfaces
│   └── Extensions.cs                       # DI registration
│
├── Domains/                                # Domain Layer
│   ├── {DomainName}/                       # e.g., Users, Sales, Products
│   │   ├── Entities/
│   │   │   └── {Entity}.cs                 # Aggregate root or entity
│   │   ├── Enums/
│   │   │   └── {EnumName}.cs
│   │   ├── Repositories/
│   │   │   └── I{Entity}Repository.cs      # Repository interface
│   │   ├── Services/
│   │   │   └── I{Service}.cs               # Domain service interface
│   │   └── ValueObjects/
│   │       └── {ValueObject}.cs
│   ├── Common/
│   │   └── Currency/                       # Shared domain concept
│   └── Shared/
│       ├── Base/
│       │   ├── BaseEntity.cs
│       │   ├── IRepository.cs
│       │   ├── PagedResult.cs
│       │   └── PagingParams.cs
│       └── ValueObjects/
│           ├── Price.cs
│           ├── Discount.cs
│           └── Weight.cs
│
├── Infrastructure/                         # Infrastructure Layer
│   ├── Data/
│   │   └── ApplicationDbContext.cs         # EF Core DbContext
│   ├── Repositories/
│   │   ├── Repository.cs                   # Base repository
│   │   └── {Entity}Repository.cs           # Concrete implementations
│   ├── Security/
│   │   └── BCryptPasswordHasher.cs
│   ├── Services/
│   │   ├── JwtTokenService.cs
│   │   └── CurrencyConversionService.cs
│   └── Extensions.cs                       # DI registration
│
├── Migrations/                             # EF Core migrations
├── Program.cs                              # Application entry point
├── appsettings.json                        # Configuration
└── retail-backend.csproj                   # Project file
```

---

## 4. Domain Layer

### 4.1 Base Entity

All entities inherit from `BaseEntity`:

```csharp
// Location: Domains/Shared/Base/BaseEntity.cs
namespace Domains.Shared.Base;

public class BaseEntity
{
    public Guid Id { get; protected set; }
    public DateTime InsertDate { get; protected set; }
    public DateTime UpdateDate { get; protected set; }
    public bool IsDeleted { get; protected set; }
    public DateTimeOffset? DeletedAt { get; protected set; }
}
```

### 4.2 Entity Structure Rules

**CRITICAL RULES:**

1. **Private parameterless constructor** for EF Core
2. **Private constructor** with all parameters
3. **Factory methods** (`Create`, `CreateXxx`) for instantiation
4. **Private setters** on all properties
5. **Domain methods** for state changes
6. **Validation in factory methods** using Value Objects

### 4.3 Aggregate Root Pattern

Each domain has ONE aggregate root:

| Domain | Aggregate Root | Child Entities |
|--------|---------------|----------------|
| Sales | `Sale` | `SaleItem` |
| Products | `Product` | `ProductPackaging` |
| Installments | `InstallmentPlan` | `InstallmentPayment` |
| Inventory | `InventoryOperation` | `InventoryOperationItem` |

**Rules:**
- External code references ONLY aggregate roots
- Child entities modified ONLY through aggregate root methods
- Aggregate ensures consistency of the entire graph

### 4.4 Value Objects

Value objects encapsulate validation and behavior:

| Value Object | Location | Purpose |
|--------------|----------|---------|
| `Email` | `Domains/Users/ValueObjects` | Email validation |
| `Password` | `Domains/Users/ValueObjects` | Password validation |
| `Phone` | `Domains/Users/ValueObjects` | Phone validation |
| `Name` | `Domains/Users/ValueObjects` | Name validation |
| `Price` | `Domains/Shared/ValueObjects` | Money with currency |
| `Discount` | `Domains/Shared/ValueObjects` | Discount calculation |
| `Weight` | `Domains/Shared/ValueObjects` | Weight measurement |
| `Barcode` | `Domains/Shared/ValueObjects` | Barcode validation |

### 4.5 Repository Interfaces

Defined in Domain layer, implemented in Infrastructure:

```csharp
// Location: Domains/{Domain}/Repositories/I{Entity}Repository.cs
namespace Domains.Users.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default);
    // Domain-specific query methods
}
```

---

## 5. Application Layer

### 5.1 CQRS Pattern

Commands (write) and Queries (read) are SEPARATE:

```
Commands (Write)              Queries (Read)
CreateSaleCommand             GetSaleByIdQuery
AddSaleItemCommand            GetSalesByOrganizationQuery
CompleteSaleCommand           GetSalesByDateRangeQuery
```

### 5.2 Command Structure

Each command has 3 files in its own folder:

```
Application/{Domain}/Commands/{Action}/
├── {Action}Command.cs           # Request definition
├── {Action}CommandHandler.cs    # Business logic
└── {Action}CommandValidator.cs  # Input validation
```

### 5.3 Query Structure

Each query has 2 files:

```
Application/{Domain}/Queries/{Query}/
├── {Query}Query.cs              # Request definition
└── {Query}QueryHandler.cs       # Data retrieval
```

### 5.4 DTOs

Located in `Application/{Domain}/DTOs/`:

- Use `record` types
- Flat structure (no navigation properties)
- Value objects mapped to primitive types
- Enums mapped to strings

### 5.5 Common Infrastructure

| Component | Location | Purpose |
|-----------|----------|---------|
| `ValidationBehavior` | `Common/Behaviors` | Auto-validates all requests |
| `ValidationException` | `Common/Exceptions` | Validation failures |
| `NotFoundException` | `Common/Exceptions` | Entity not found |
| `UnauthorizedException` | `Common/Exceptions` | Auth failures |
| `MappingProfile` | `Common/Mappings` | AutoMapper configuration |
| `ApiResponse<T>` | `Common/Models` | Success response wrapper |
| `ApiErrorResponse` | `Common/Models` | Error response wrapper |

---

## 6. Infrastructure Layer

### 6.1 Base Repository

```csharp
// Location: Infrastructure/Repositories/Repository.cs
public abstract class Repository<TEntity> : IRepository<TEntity>
    where TEntity : BaseEntity
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<TEntity> _dbSet;

    protected Repository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    // Common CRUD operations
}
```

### 6.2 Concrete Repository Pattern

```csharp
// Location: Infrastructure/Repositories/{Entity}Repository.cs
public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context) { }

    // Domain-specific implementations
    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        return await _dbSet
            .AsNoTracking()  // IMPORTANT: Always use for reads
            .FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant(), ct);
    }
}
```

### 6.3 Service Implementations

Services implement interfaces defined in Domain or Application:

| Interface | Implementation | Layer |
|-----------|----------------|-------|
| `IPasswordHasher` | `BCryptPasswordHasher` | Domain → Infrastructure |
| `ICurrencyConversionService` | `CurrencyConversionService` | Domain → Infrastructure |
| `IJwtTokenService` | `JwtTokenService` | Application → Infrastructure |

---

## 7. API Layer

### 7.1 Controller Pattern

```csharp
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
}
```

### 7.2 Global Exception Filter

All exceptions are caught and converted to standardized responses:

| Exception Type | HTTP Status | Message |
|----------------|-------------|---------|
| `ValidationException` | 400 | فشل التحقق من الصحة |
| `UnauthorizedException` | 401 | (exception message) |
| `NotFoundException` | 404 | (exception message) |
| Other | 500 | حدث خطأ في الخادم |

### 7.3 Authentication

JWT Bearer authentication configured in `API/Configuration/AuthenticationConfiguration.cs`:

```csharp
services.AddJwtAuthentication(builder.Configuration);
```

Configuration in `appsettings.json`:

```json
{
  "Jwt": {
    "Secret": "your-secret-key-min-32-chars",
    "Issuer": "RetailMatrix",
    "Audience": "RetailMatrixApp",
    "ExpirationMinutes": 60
  }
}
```

---

## 8. Dependency Injection

### 8.1 Program.cs Setup

```csharp
// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Application Layer (MediatR, AutoMapper, FluentValidation)
builder.Services.AddApplication();

// Infrastructure Layer (Repositories, Services)
builder.Services.AddInfrastructure();

// JWT Authentication
builder.Services.AddJwtAuthentication(builder.Configuration);

// Controllers with exception filter
builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalExceptionFilter>();
});
```

### 8.2 Application Extensions

```csharp
// Location: Application/Extensions.cs
public static IServiceCollection AddApplication(this IServiceCollection services)
{
    var assembly = Assembly.GetExecutingAssembly();

    services.AddMediatR(assembly);
    services.AddAutoMapper(assembly);
    services.AddValidatorsFromAssembly(assembly);
    services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

    return services;
}
```

### 8.3 Infrastructure Extensions

```csharp
// Location: Infrastructure/Extensions.cs
public static IServiceCollection AddInfrastructure(this IServiceCollection services)
{
    services.AddMemoryCache();

    // Domain Services
    services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
    services.AddScoped<ICurrencyConversionService, CurrencyConversionService>();
    services.AddScoped<IJwtTokenService, JwtTokenService>();

    // Repositories
    services.AddScoped<IUserRepository, UserRepository>();
    // ... other repositories

    return services;
}
```

---

## 9. Database & EF Core

### 9.1 DbContext Configuration

- Soft delete handled automatically
- Value objects configured as owned types
- Enums stored as strings

### 9.2 Migration Commands

```bash
# Create migration
dotnet ef migrations add MigrationName

# Update database
dotnet ef database update

# Remove last migration
dotnet ef migrations remove
```

### 9.3 Connection String

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=retail_matrix;User=root;Password=password;"
  }
}
```

---

## Quick Reference Card

| What | Where | Pattern |
|------|-------|---------|
| New Entity | `Domains/{Domain}/Entities/` | Private ctor + Factory method |
| New Repository | `Domains/{Domain}/Repositories/` + `Infrastructure/Repositories/` | Interface + Implementation |
| New Command | `Application/{Domain}/Commands/{Action}/` | Command + Handler + Validator |
| New Query | `Application/{Domain}/Queries/{Query}/` | Query + Handler |
| New DTO | `Application/{Domain}/DTOs/` | record type |
| New Value Object | `Domains/{Domain}/ValueObjects/` or `Domains/Shared/ValueObjects/` | Sealed class + Create() |
| New Controller | `API/Controllers/` | Uses IMediator |
| Register Service | `Infrastructure/Extensions.cs` | AddScoped/AddSingleton |

---

**Next:** See [CODING_PATTERNS.md](CODING_PATTERNS.md) for detailed code examples.
