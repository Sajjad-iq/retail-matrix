# Quick Start - Application Layer Implementation

> **Purpose:** Step-by-step guide to implement CQRS Application Layer with MediatR

## Prerequisites

- .NET 9 SDK installed
- Existing Domain and Infrastructure layers
- MySQL database configured

---

## 🚀 Implementation Steps

### Step 1: Install NuGet Packages

```bash
cd /home/sajjad/Documents/retail-matrix/retail-backend
dotnet add package MediatR
dotnet add package MediatR.Extensions.Microsoft.DependencyInjection
dotnet add package FluentValidation
dotnet add package FluentValidation.DependencyInjectionExtensions
dotnet add package AutoMapper
dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection
```


### Step 2: Create Folder Structure

```bash
# Base structure
mkdir -p Application/{Sales,Installments,Products}/{Commands,Queries,DTOs}
mkdir -p Application/Common/{Behaviors,Exceptions,Mappings}

# Sales commands/queries
mkdir -p Application/Sales/Commands/{CreateSale,AddSaleItem,CompleteSale,RecordPayment}
mkdir -p Application/Sales/Queries/{GetSaleById,GetSalesByOrganization}

# Installments commands/queries
mkdir -p Application/Installments/Commands/{CreateInstallmentPlan,GeneratePaymentSchedule,RecordInstallmentPayment,ActivateInstallmentPlan}
mkdir -p Application/Installments/Queries/{GetInstallmentPlanById,GetInstallmentPlansBySale,GetInstallmentPlansByCustomer}
```


### Step 3: Create Common Infrastructure

#### `Application/Common/Exceptions/NotFoundException.cs`
```csharp
namespace Application.Common.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}
```

#### `Application/Common/Exceptions/ValidationException.cs`
```csharp
using FluentValidation.Results;

namespace Application.Common.Exceptions;

public class ValidationException : Exception
{
    public List<string> Errors { get; }

    public ValidationException(IEnumerable<ValidationFailure> failures)
        : base("حدثت أخطاء في التحقق من الصحة")
    {
        Errors = failures.Select(f => f.ErrorMessage).ToList();
    }
}
```

#### `Application/Common/Behaviors/ValidationBehavior.cs`
```csharp
using FluentValidation;
using MediatR;

namespace Application.Common.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Any())
            throw new ValidationException(failures);

        return await next();
    }
}
```

### Step 4: Create AutoMapper Profile

#### `Application/Common/Mappings/MappingProfile.cs`
```csharp
using Application.Sales.DTOs;
using Application.Installments.DTOs;
using AutoMapper;
using Domains.Sales.Entities;
using Domains.Installments.Entities;

namespace Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Sales mappings
        CreateMap<Sale, SaleDto>()
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.TotalDiscount, opt => opt.MapFrom(s => s.TotalDiscount.Amount))
            .ForMember(d => d.TotalDiscountCurrency, opt => opt.MapFrom(s => s.TotalDiscount.Currency))
            .ForMember(d => d.GrandTotal, opt => opt.MapFrom(s => s.GrandTotal.Amount))
            .ForMember(d => d.GrandTotalCurrency, opt => opt.MapFrom(s => s.GrandTotal.Currency))
            .ForMember(d => d.AmountPaid, opt => opt.MapFrom(s => s.AmountPaid.Amount))
            .ForMember(d => d.AmountPaidCurrency, opt => opt.MapFrom(s => s.AmountPaid.Currency));

        CreateMap<SaleItem, SaleItemDto>()
            .ForMember(d => d.UnitPrice, opt => opt.MapFrom(s => s.UnitPrice.Amount))
            .ForMember(d => d.UnitPriceCurrency, opt => opt.MapFrom(s => s.UnitPrice.Currency))
            .ForMember(d => d.LineTotal, opt => opt.MapFrom(s => s.LineTotal.Amount))
            .ForMember(d => d.LineTotalCurrency, opt => opt.MapFrom(s => s.LineTotal.Currency));

        // Installment mappings
        CreateMap<InstallmentPlan, InstallmentPlanDto>()
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.PaymentFrequency, opt => opt.MapFrom(s => s.PaymentFrequency.ToString()))
            .ForMember(d => d.OriginalAmount, opt => opt.MapFrom(s => s.OriginalAmount.Amount))
            .ForMember(d => d.Currency, opt => opt.MapFrom(s => s.OriginalAmount.Currency))
            .ForMember(d => d.DownPayment, opt => opt.MapFrom(s => s.DownPayment.Amount))
            .ForMember(d => d.InterestAmount, opt => opt.MapFrom(s => s.InterestAmount.Amount));

        CreateMap<InstallmentPayment, InstallmentPaymentDto>()
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.DueAmount, opt => opt.MapFrom(s => s.DueAmount.Amount))
            .ForMember(d => d.PaidAmount, opt => opt.MapFrom(s => s.PaidAmount.Amount))
            .ForMember(d => d.Currency, opt => opt.MapFrom(s => s.DueAmount.Currency));
    }
}
```

### Step 5: Register Application Services

#### `Application/Extensions.cs`
```csharp
using System.Reflection;
using Application.Common.Behaviors;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class Extensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

        // AutoMapper
        services.AddAutoMapper(assembly);

        // FluentValidation
        services.AddValidatorsFromAssembly(assembly);

        // Pipeline Behaviors
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}
```

### Step 6: Create DTOs

#### `Application/Sales/DTOs/SaleDto.cs`
```csharp
namespace Application.Sales.DTOs;

public record SaleDto
{
    public Guid Id { get; init; }
    public string SaleNumber { get; init; } = string.Empty;
    public DateTime SaleDate { get; init; }
    public Guid SalesPersonId { get; init; }
    public Guid OrganizationId { get; init; }
    public string Status { get; init; } = string.Empty;
    public decimal TotalDiscount { get; init; }
    public string TotalDiscountCurrency { get; init; } = string.Empty;
    public decimal GrandTotal { get; init; }
    public string GrandTotalCurrency { get; init; } = string.Empty;
    public decimal AmountPaid { get; init; }
    public string AmountPaidCurrency { get; init; } = string.Empty;
    public string? Notes { get; init; }
    public List<SaleItemDto> Items { get; init; } = new();
}

public record SaleItemDto
{
    public Guid Id { get; init; }
    public Guid ProductPackagingId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public string UnitPriceCurrency { get; init; } = string.Empty;
    public decimal LineTotal { get; init; }
    public string LineTotalCurrency { get; init; } = string.Empty;
}
```

#### `Application/Installments/DTOs/InstallmentPlanDto.cs`
```csharp
namespace Application.Installments.DTOs;

public record InstallmentPlanDto
{
    public Guid Id { get; init; }
    public string PlanNumber { get; init; } = string.Empty;
    public Guid CustomerId { get; init; }
    public Guid SaleId { get; init; }
    public decimal OriginalAmount { get; init; }
    public decimal DownPayment { get; init; }
    public decimal InterestAmount { get; init; }
    public string Currency { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string PaymentFrequency { get; init; } = string.Empty;
    public decimal TotalToPay { get; init; }
    public decimal TotalPaid { get; init; }
    public decimal RemainingBalance { get; init; }
    public decimal ProgressPercentage { get; init; }
    public List<InstallmentPaymentDto> Payments { get; init; } = new();
}

public record InstallmentPaymentDto
{
    public Guid Id { get; init; }
    public int InstallmentNumber { get; init; }
    public DateTime DueDate { get; init; }
    public decimal DueAmount { get; init; }
    public decimal PaidAmount { get; init; }
    public string Currency { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTime? PaidDate { get; init; }
}
```

### Step 7: Create First Command (CreateSale)

#### `Application/Sales/Commands/CreateSale/CreateSaleCommand.cs`
```csharp
using MediatR;

namespace Application.Sales.Commands.CreateSale;

public record CreateSaleCommand : IRequest<Guid>
{
    public Guid OrganizationId { get; init; }
    public Guid SalesPersonId { get; init; }
}
```

#### `Application/Sales/Commands/CreateSale/CreateSaleCommandValidator.cs`
```csharp
using FluentValidation;

namespace Application.Sales.Commands.CreateSale;

public class CreateSaleCommandValidator : AbstractValidator<CreateSaleCommand>
{
    public CreateSaleCommandValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty()
            .WithMessage("معرف المنظمة مطلوب");

        RuleFor(x => x.SalesPersonId)
            .NotEmpty()
            .WithMessage("معرف البائع مطلوب");
    }
}
```

#### `Application/Sales/Commands/CreateSale/CreateSaleCommandHandler.cs`
```csharp
using Domains.Sales.Entities;
using Domains.Sales.Repositories;
using MediatR;

namespace Application.Sales.Commands.CreateSale;

public class CreateSaleCommandHandler : IRequestHandler<CreateSaleCommand, Guid>
{
    private readonly ISaleRepository _saleRepository;

    public CreateSaleCommandHandler(ISaleRepository saleRepository)
    {
        _saleRepository = saleRepository;
    }

    public async Task<Guid> Handle(CreateSaleCommand request, CancellationToken cancellationToken)
    {
        var sale = Sale.Create(request.OrganizationId, request.SalesPersonId);

        await _saleRepository.AddAsync(sale, cancellationToken);
        await _saleRepository.SaveChangesAsync(cancellationToken);

        return sale.Id;
    }
}
```

### Step 8: Create First Query (GetSaleById)

#### `Application/Sales/Queries/GetSaleById/GetSaleByIdQuery.cs`
```csharp
using Application.Sales.DTOs;
using MediatR;

namespace Application.Sales.Queries.GetSaleById;

public record GetSaleByIdQuery(Guid SaleId) : IRequest<SaleDto?>;
```

#### `Application/Sales/Queries/GetSaleById/GetSaleByIdQueryHandler.cs`
```csharp
using Application.Common.Exceptions;
using Application.Sales.DTOs;
using AutoMapper;
using Domains.Sales.Repositories;
using MediatR;

namespace Application.Sales.Queries.GetSaleById;

public class GetSaleByIdQueryHandler : IRequestHandler<GetSaleByIdQuery, SaleDto?>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    public GetSaleByIdQueryHandler(ISaleRepository saleRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    public async Task<SaleDto?> Handle(GetSaleByIdQuery request, CancellationToken cancellationToken)
    {
        var sale = await _saleRepository.GetByIdAsync(request.SaleId, cancellationToken);
        
        if (sale == null)
            throw new NotFoundException($"Sale with ID {request.SaleId} not found");

        return _mapper.Map<SaleDto>(sale);
    }
}
```

### Step 9: Update Program.cs

```csharp
using Application;
using Infrastructure;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Application Layer (NEW!)
builder.Services.AddApplication();

// Infrastructure Layer
builder.Services.AddInfrastructure();

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();
```

### Step 10: Register Missing Repositories

Update `Infrastructure/Extensions.cs` to include missing repositories:

```csharp
// Add these lines to Infrastructure/Extensions.cs
services.AddScoped<ISaleRepository, SaleRepository>();
services.AddScoped<IInstallmentPlanRepository, InstallmentPlanRepository>();
services.AddScoped<ICustomerRepository, CustomerRepository>();
```

---


## ✅ Implementation Checklist

- [ ] Install NuGet packages (MediatR, FluentValidation, AutoMapper)
- [ ] Create folder structure
- [ ] Create common exceptions (NotFoundException, ValidationException)
- [ ] Create ValidationBehavior
- [ ] Create MappingProfile (AutoMapper)
- [ ] Create Extensions.cs (DI registration)
- [ ] Create DTOs (SaleDto, InstallmentPlanDto)
- [ ] Create CreateSaleCommand + Handler + Validator
- [ ] Create GetSaleByIdQuery + Handler
- [ ] Update Program.cs to register Application layer
- [ ] Update Infrastructure/Extensions.cs (add missing repositories)
- [ ] Build and verify (`dotnet build`)

---


## 🧪 Verify Implementation

```bash
# Build the project
dotnet build

# Expected output: Build succeeded
```

**If build succeeds:** You're ready to implement more commands and queries!

**If build fails:** Check for missing using statements or repository registrations.

---


## 📋 Next Steps

### Recommended Implementation Order

**1. Sales Domain (Priority: High)**
- [x] CreateSale
- [ ] AddSaleItem
- [ ] UpdateSaleItemQuantity
- [ ] RemoveSaleItem
- [ ] RecordPayment
- [ ] CompleteSale
- [ ] CancelSale

**2. Installments Domain (Priority: High)**
- [ ] CreateInstallmentPlan
- [ ] GeneratePaymentSchedule
- [ ] ActivateInstallmentPlan
- [ ] RecordInstallmentPayment

**3. Products Domain (Priority: Medium)**
- [ ] CreateProduct
- [ ] AddProductPackaging
- [ ] UpdateProduct
- [ ] ActivateProduct / DeactivateProduct

**4. Inventory & Stocks (Priority: Medium)**
- [ ] CreateInventoryOperation
- [ ] AddInventoryOperationItem
- [ ] CompleteInventoryOperation

---

**Pattern to Follow:** Each command/query follows the same structure as CreateSale. Copy the pattern and adapt for your use case!
