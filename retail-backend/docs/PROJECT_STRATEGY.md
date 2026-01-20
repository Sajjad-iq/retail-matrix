# Retail Matrix - Architecture & Strategy Reference

> **Purpose:** Reference guide for Domain-Driven Design and CQRS implementation patterns

## 📋 Table of Contents

1. [Project Overview](#project-overview)
2. [Architecture Layers](#architecture-layers)
3. [Domain-Driven Design Patterns](#domain-driven-design-patterns)
4. [Application Layer (CQRS)](#application-layer-cqrs)
5. [Coding Standards](#coding-standards)
6. [Best Practices](#best-practices)

---

## 🎯 Project Overview

**Retail Matrix** is a multi-tenant retail management system built with **Domain-Driven Design (DDD)** principles and **Clean Architecture**.

### Core Business Domains

- **Sales** - Point of sale transactions
- **Installments** - Payment plans and schedules
- **Products** - Product catalog with packaging variants
- **Inventory** - Warehouse and stock operations
- **Stocks** - Stock levels and batch tracking
- **Users** - Authentication and user management
- **Organizations** - Multi-tenant support
- **Currency** - Multi-currency support with conversion

### Technology Stack

- **.NET 9** - Framework
- **Entity Framework Core** - ORM
- **MySQL** - Database
- **MediatR** - CQRS implementation
- **FluentValidation** - Input validation
- **AutoMapper** - Object mapping
- **BCrypt.Net** - Password hashing

---

## 🏗️ Architecture Layers

```
retail-backend/
├── Domains/              # Domain Layer (Business Logic)
├── Application/          # Application Layer (Use Cases) - TO BE IMPLEMENTED
├── Infrastructure/       # Infrastructure Layer (Data Access)
├── API/                  # Presentation Layer (REST API) - TO BE IMPLEMENTED
└── Tests/                # Test Projects - TO BE IMPLEMENTED
```

### Layer Responsibilities

| Layer | Responsibility | Dependencies |
|-------|---------------|--------------|
| **Domain** | Business entities, rules, interfaces | None (pure business logic) |
| **Application** | Use cases, commands, queries, DTOs | Domain |
| **Infrastructure** | Repositories, external services, DB | Domain, Application |
| **API** | Controllers, middleware, authentication | Application |

### Dependency Rule

**Dependencies flow inward only:**
```
API → Application → Domain
     ↓
Infrastructure → Domain
```

---

## 🎨 Domain-Driven Design Patterns

### 1. Rich Domain Models

✅ **DO:**
```csharp
public class Sale : BaseEntity
{
    private Sale() { } // EF Core constructor
    
    private Sale(string saleNumber, Guid organizationId, Guid salesPersonId)
    {
        // Initialize with valid state
    }
    
    public static Sale Create(Guid organizationId, Guid salesPersonId)
    {
        // Factory method enforces business rules
        var saleNumber = GenerateSaleNumber();
        return new Sale(saleNumber, organizationId, salesPersonId);
    }
    
    public void CompleteSale()
    {
        if (Status == SaleStatus.Completed)
            throw new InvalidOperationException("البيع مكتمل بالفعل");
        
        if (Items.Count == 0)
            throw new InvalidOperationException("لا يمكن إكمال بيع بدون عناصر");
            
        Status = SaleStatus.Completed;
    }
}
```

❌ **DON'T:**
```csharp
public class Sale
{
    public Guid Id { get; set; }
    public string SaleNumber { get; set; }
    public SaleStatus Status { get; set; }
    // Anemic model - no behavior, just data
}
```

### 2. Value Objects

Use value objects for domain concepts:

```csharp
public class Price
{
    public decimal Amount { get; }
    public string Currency { get; }
    
    private Price(decimal amount, string currency)
    {
        if (amount < 0)
            throw new ArgumentException("السعر لا يمكن أن يكون سالباً");
        
        Amount = amount;
        Currency = currency;
    }
    
    public static Price Create(decimal amount, string currency) 
        => new Price(amount, currency);
    
    public Price Add(Price other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("لا يمكن جمع أسعار بعملات مختلفة");
        
        return new Price(Amount + other.Amount, Currency);
    }
}
```

### 3. Aggregate Roots

Each domain has clear aggregate roots:

- **Sale** (aggregate root) → SaleItem (entity)
- **InstallmentPlan** (aggregate root) → InstallmentPayment (entity)
- **Product** (aggregate root) → ProductPackaging (entity)
- **InventoryOperation** (aggregate root) → InventoryOperationItem (entity)

**Rules:**
- External references only to aggregate roots
- Modifications only through aggregate root methods
- Aggregate ensures consistency

### 4. Repository Pattern

One repository per aggregate root:

```csharp
public interface ISaleRepository : IRepository<Sale>
{
    Task<Sale?> GetBySaleNumberAsync(string saleNumber, CancellationToken ct = default);
    Task<PagedResult<Sale>> GetByOrganizationAsync(Guid orgId, PagingParams paging, CancellationToken ct = default);
    Task<decimal> GetTotalSalesAmountAsync(Guid orgId, DateTime start, DateTime end, CancellationToken ct = default);
}
```

### 5. Domain Services

For operations that don't belong to a single entity:

```csharp
public interface ICurrencyConversionService
{
    Task<decimal> ConvertToBaseCurrencyAsync(decimal amount, string fromCurrency, Guid organizationId, CancellationToken ct = default);
    Task<string> GetBaseCurrencyCodeAsync(Guid organizationId, CancellationToken ct = default);
}
```

---

## 🚀 Application Layer (CQRS)

### CQRS Pattern with MediatR

**Separate reads from writes:**

```
Commands (Write)          Queries (Read)
     ↓                         ↓
  Handlers                 Handlers
     ↓                         ↓
  Domain                   Repository
     ↓                         ↓
  Repository                  DTOs
```

### Folder Structure

```
Application/
├── Sales/
│   ├── Commands/
│   │   ├── CreateSale/
│   │   │   ├── CreateSaleCommand.cs
│   │   │   ├── CreateSaleCommandHandler.cs
│   │   │   └── CreateSaleCommandValidator.cs
│   │   ├── AddSaleItem/
│   │   ├── CompleteSale/
│   │   └── RecordPayment/
│   ├── Queries/
│   │   ├── GetSaleById/
│   │   │   ├── GetSaleByIdQuery.cs
│   │   │   └── GetSaleByIdQueryHandler.cs
│   │   ├── GetSalesByOrganization/
│   │   └── GetSalesByDateRange/
│   └── DTOs/
│       ├── SaleDto.cs
│       └── SaleItemDto.cs
├── Installments/
│   ├── Commands/
│   ├── Queries/
│   └── DTOs/
├── Products/
│   ├── Commands/
│   ├── Queries/
│   └── DTOs/
└── Common/
    ├── Behaviors/
    │   ├── ValidationBehavior.cs
    │   ├── LoggingBehavior.cs
    │   └── TransactionBehavior.cs
    ├── Exceptions/
    │   ├── ValidationException.cs
    │   ├── NotFoundException.cs
    │   └── BusinessRuleException.cs
    └── Mappings/
        └── MappingProfile.cs
```

### Command Example

```csharp
// Command (Request)
public record CreateSaleCommand : IRequest<Guid>
{
    public Guid OrganizationId { get; init; }
    public Guid SalesPersonId { get; init; }
}

// Validator
public class CreateSaleCommandValidator : AbstractValidator<CreateSaleCommand>
{
    public CreateSaleCommandValidator()
    {
        RuleFor(x => x.OrganizationId).NotEmpty().WithMessage("معرف المنظمة مطلوب");
        RuleFor(x => x.SalesPersonId).NotEmpty().WithMessage("معرف البائع مطلوب");
    }
}

// Handler
public class CreateSaleCommandHandler : IRequestHandler<CreateSaleCommand, Guid>
{
    private readonly ISaleRepository _saleRepository;

    public CreateSaleCommandHandler(ISaleRepository saleRepository)
    {
        _saleRepository = saleRepository;
    }

    public async Task<Guid> Handle(CreateSaleCommand request, CancellationToken ct)
    {
        // 1. Create domain entity using factory method
        var sale = Sale.Create(request.OrganizationId, request.SalesPersonId);

        // 2. Persist
        await _saleRepository.AddAsync(sale, ct);
        await _saleRepository.SaveChangesAsync(ct);

        // 3. Return identifier
        return sale.Id;
    }
}
```

### Query Example

```csharp
// Query (Request)
public record GetSaleByIdQuery(Guid SaleId) : IRequest<SaleDto?>;

// DTO (Response)
public record SaleDto
{
    public Guid Id { get; init; }
    public string SaleNumber { get; init; } = string.Empty;
    public DateTime SaleDate { get; init; }
    public string Status { get; init; } = string.Empty;
    public decimal GrandTotal { get; init; }
    public string Currency { get; init; } = string.Empty;
    public List<SaleItemDto> Items { get; init; } = new();
}

// Handler
public class GetSaleByIdQueryHandler : IRequestHandler<GetSaleByIdQuery, SaleDto?>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    public GetSaleByIdQueryHandler(ISaleRepository saleRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    public async Task<SaleDto?> Handle(GetSaleByIdQuery request, CancellationToken ct)
    {
        var sale = await _saleRepository.GetByIdAsync(request.SaleId, ct);
        
        if (sale == null)
            throw new NotFoundException($"Sale with ID {request.SaleId} not found");

        return _mapper.Map<SaleDto>(sale);
    }
}
```

### Pipeline Behaviors

**Automatic validation for all commands:**

```csharp
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        if (!_validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, ct)));
        var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();

        if (failures.Any())
            throw new ValidationException(failures);

        return await next();
    }
}
```

## 📝 Coding Standards

### Naming Conventions

| Type | Convention | Example |
|------|-----------|---------|
| Commands | `{Verb}{Entity}Command` | `CreateSaleCommand` |
| Queries | `Get{Entity}By{Criteria}Query` | `GetSaleByIdQuery` |
| Handlers | `{Request}Handler` | `CreateSaleCommandHandler` |
| Validators | `{Request}Validator` | `CreateSaleCommandValidator` |
| DTOs | `{Entity}Dto` | `SaleDto` |
| Repositories | `I{Entity}Repository` | `ISaleRepository` |

### Command/Query Guidelines

✅ **Commands:**
- Return only identifiers or void
- Modify state
- Use validators
- Wrap in transactions

✅ **Queries:**
- Return DTOs, never domain entities
- Read-only (AsNoTracking)
- Can bypass domain layer for performance
- No side effects

### Error Handling

```csharp
// Domain exceptions
throw new InvalidOperationException("البيع مكتمل بالفعل");

// Application exceptions
throw new NotFoundException($"Sale with ID {id} not found");
throw new ValidationException(failures);
throw new BusinessRuleException("المبلغ المدفوع يتجاوز الإجمالي المطلوب");
```

### Validation Rules

```csharp
public class AddSaleItemCommandValidator : AbstractValidator<AddSaleItemCommand>
{
    public AddSaleItemCommandValidator()
    {
        RuleFor(x => x.SaleId)
            .NotEmpty()
            .WithMessage("معرف البيع مطلوب");

        RuleFor(x => x.ProductPackagingId)
            .NotEmpty()
            .WithMessage("معرف المنتج مطلوب");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("الكمية يجب أن تكون أكبر من صفر");

        RuleFor(x => x.UnitPrice)
            .GreaterThan(0)
            .WithMessage("السعر يجب أن يكون أكبر من صفر");
    }
}
```


---

---

## 🎯 Best Practices

### ✅ DO:

1. **Keep domain logic in domain entities** - not in handlers
2. **Use factory methods** for entity creation
3. **Validate in application layer** with FluentValidation
4. **Return DTOs from queries** - never domain entities
5. **Use value objects** for domain concepts (Price, Email, etc.)
6. **One handler per command/query** - Single Responsibility
7. **Use meaningful Arabic error messages** - user-facing
8. **Wrap commands in transactions** - via TransactionBehavior
9. **Use AsNoTracking for queries** - performance
10. **Test domain logic thoroughly** - unit tests

### ❌ DON'T:

1. **Don't put business logic in handlers** - belongs in domain
2. **Don't inject DbContext in handlers** - use repositories
3. **Don't return domain entities from API** - use DTOs
4. **Don't skip validation** - always validate commands
5. **Don't create anemic domain models** - add behavior
6. **Don't use navigation properties across domains** - loose coupling
7. **Don't bypass aggregate roots** - maintain consistency
8. **Don't use EF tracking for queries** - performance hit
9. **Don't create god classes** - keep handlers focused
10. **Don't ignore exceptions** - handle gracefully

---

## 📦 Required NuGet Packages

### Application Layer
```bash
dotnet add package MediatR
dotnet add package MediatR.Extensions.Microsoft.DependencyInjection
dotnet add package FluentValidation
dotnet add package FluentValidation.DependencyInjectionExtensions
dotnet add package AutoMapper
dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection
```

### Infrastructure Layer (Already Installed)
- Microsoft.EntityFrameworkCore
- Pomelo.EntityFrameworkCore.MySql
- BCrypt.Net-Next

### Future Packages
- **API:** Microsoft.AspNetCore.Authentication.JwtBearer, Swashbuckle.AspNetCore
- **Testing:** xUnit, Moq, FluentAssertions
