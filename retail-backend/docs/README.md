# Retail Matrix - Documentation

> **FOR AI ASSISTANTS:** Before implementing ANY new feature, you MUST read both documentation files below. These documents define the EXACT patterns and conventions used in this codebase. Failure to follow these patterns will result in inconsistent code.

---

## Documentation Files

### 1. [ARCHITECTURE.md](ARCHITECTURE.md)

**Read this FIRST to understand:**

- Project structure and folder organization
- Layer responsibilities (Domain, Application, Infrastructure, API)
- Dependency rules and injection patterns
- Database and EF Core configuration
- Authentication setup

**Key sections:**
- Layer Architecture diagram
- Complete folder structure
- Domain layer rules (entities, value objects, repositories)
- Application layer rules (CQRS pattern)
- Infrastructure and API layer patterns

---

### 2. [CODING_PATTERNS.md](CODING_PATTERNS.md)

**Read this to implement new features:**

- Complete code templates for every component type
- Real code examples extracted from this codebase
- Step-by-step patterns for:
  - Entities (aggregate roots and children)
  - Value objects
  - Repositories (interface + implementation)
  - Commands (Command + Handler + Validator)
  - Queries (Query + Handler)
  - DTOs
  - Controllers
  - Services
  - Exception handling
  - AutoMapper configuration

**Key sections:**
- Entity pattern with real `User` and `Sale` examples
- Value object pattern with real `Email` and `Price` examples
- CQRS patterns with real `Register` and `Login` examples
- Naming conventions table
- Arabic error messages reference

---

## Quick Reference

### When Adding a New Feature

1. **Read** [ARCHITECTURE.md](ARCHITECTURE.md) to understand where files go
2. **Read** [CODING_PATTERNS.md](CODING_PATTERNS.md) to copy the exact patterns
3. **Create files** following the folder structure
4. **Register services** in the appropriate `Extensions.cs`

### Folder Structure Summary

```
retail-backend/
├── API/Controllers/                    # REST endpoints
├── Application/{Domain}/
│   ├── Commands/{Action}/              # Command + Handler + Validator
│   ├── Queries/{Query}/                # Query + Handler
│   └── DTOs/                           # Data transfer objects
├── Domains/{Domain}/
│   ├── Entities/                       # Aggregate roots and entities
│   ├── Enums/                          # Domain enums
│   ├── Repositories/                   # Repository interfaces
│   ├── Services/                       # Domain service interfaces
│   └── ValueObjects/                   # Value objects
└── Infrastructure/
    ├── Repositories/                   # Repository implementations
    └── Services/                       # Service implementations
```

### Naming Convention Summary

| Component | Pattern | Example |
|-----------|---------|---------|
| Entity | `{Name}` | `User`, `Sale` |
| Command | `{Verb}{Entity}Command` | `CreateSaleCommand` |
| Query | `Get{Entity}By{X}Query` | `GetSaleByIdQuery` |
| Handler | `{Request}Handler` | `CreateSaleCommandHandler` |
| Validator | `{Request}Validator` | `CreateSaleCommandValidator` |
| DTO | `{Entity}Dto` | `SaleDto` |
| Repository | `I{Entity}Repository` | `ISaleRepository` |

---

## Important Rules

### DO:
- Use factory methods for entity creation (`Entity.Create()`)
- Use private setters on entity properties
- Use value objects for domain concepts (Email, Price, etc.)
- Validate in Application layer with FluentValidation
- Return DTOs from queries, never entities
- Use Arabic error messages
- Use `AsNoTracking()` for all read queries

### DON'T:
- Create anemic entities (entities without behavior)
- Put business logic in handlers (belongs in domain)
- Return domain entities from API
- Skip validation
- Modify child entities directly (go through aggregate root)
- Use public setters on entities

---

## Running the Application

```bash
cd /home/sajjad/Documents/retail-matrix/retail-backend
dotnet run
```

**Available Endpoints:**
- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - Login and get JWT
- `GET /api/auth/me` - Get current user (requires auth)

---

## Database Commands

```bash
# Create migration
dotnet ef migrations add MigrationName

# Update database
dotnet ef database update

# Remove last migration
dotnet ef migrations remove
```

---

**Start with [ARCHITECTURE.md](ARCHITECTURE.md), then use [CODING_PATTERNS.md](CODING_PATTERNS.md) as your implementation guide.**
