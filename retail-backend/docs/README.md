# Documentation Index

## 📚 Available Guides

### 1. [PROJECT_STRATEGY.md](PROJECT_STRATEGY.md)
**Purpose:** Architecture and design patterns reference

**Contains:**
- Domain-Driven Design patterns
- CQRS architecture with MediatR
- Coding standards and conventions
- Best practices (DO's and DON'Ts)
- Required NuGet packages

**Use when:** You need to understand the architecture or look up design patterns

---

### 2. [QUICK_START.md](QUICK_START.md)
**Purpose:** Step-by-step implementation guide

**Contains:**
- Installation instructions
- Folder structure setup
- Complete code examples for:
  - Common infrastructure (exceptions, behaviors, mappings)
  - DTOs (Data Transfer Objects)
  - Commands and Queries
  - Handlers and Validators
- Implementation checklist
- Next steps roadmap

**Use when:** You're ready to start implementing the Application layer

---

## 🚀 Getting Started

1. **Read** [PROJECT_STRATEGY.md](PROJECT_STRATEGY.md) to understand the architecture
2. **Follow** [QUICK_START.md](QUICK_START.md) to implement the Application layer
3. **Refer back** to PROJECT_STRATEGY.md for patterns and best practices

---

## 📁 Project Structure

```
retail-backend/
├── Domains/              ✅ Implemented (Business Logic)
├── Infrastructure/       ✅ Implemented (Data Access)
├── Application/          ⏳ To Be Implemented (Use Cases)
├── API/                  ⏳ Future (REST API)
└── docs/                 📚 This folder
    ├── README.md         (This file)
    ├── PROJECT_STRATEGY.md
    └── QUICK_START.md
```

---

## 🎯 Current Status

- ✅ **Domain Layer** - Complete with rich domain models
- ✅ **Infrastructure Layer** - Complete with repositories and EF Core
- ⏳ **Application Layer** - Ready to implement (follow QUICK_START.md)
- ⏳ **API Layer** - Future phase

---

## 💡 Quick Reference

### Key Concepts

- **Aggregate Root:** Main entity that controls access to related entities
- **Value Object:** Immutable object representing a domain concept (Price, Email)
- **Repository:** Interface for data access, one per aggregate root
- **Command:** Write operation (creates, updates, deletes)
- **Query:** Read operation (returns DTOs)
- **DTO:** Data Transfer Object for API responses
- **Handler:** Processes a command or query

### Naming Conventions

| Type | Pattern | Example |
|------|---------|---------|
| Command | `{Verb}{Entity}Command` | `CreateSaleCommand` |
| Query | `Get{Entity}By{Criteria}Query` | `GetSaleByIdQuery` |
| Handler | `{Request}Handler` | `CreateSaleCommandHandler` |
| Validator | `{Request}Validator` | `CreateSaleCommandValidator` |
| DTO | `{Entity}Dto` | `SaleDto` |

---

**Need help?** Refer to the specific guide based on what you need to do!
