# Project Folder Structure

This document describes the folder organization of this Next.js starter project.

## 📁 Root Structure

```
nextjs-starter/
├── app/                    # Next.js App Router (pages & routing)
├── components/             # Shared UI components
├── features/              # Feature modules (domain-driven)
├── lib/                   # Core utilities & configuration
├── hooks/                 # Shared custom hooks
├── stores/                # Global state (Zustand)
├── services/              # Global API services
├── i18n/                  # Internationalization
└── public/                # Static assets
```

---

## 📂 Detailed Structure

### `app/` - Next.js App Router
Pages, layouts, and API routes.

```
app/
├── (auth)/                # Auth pages (login, register)
├── (main)/                # Main app pages (dashboard, settings, users)
├── api/                   # API route handlers
├── layout.tsx             # Root layout
├── providers.tsx          # Global providers (React Query, i18n, theme)
└── globals.css            # Global styles
```

**Purpose**: Route definitions and page-level components only.

---

### `features/` - Feature Modules
Self-contained feature modules following domain-driven design.

```
features/
├── auth/
│   ├── components/        # Auth-specific components (AuthCard)
│   ├── hooks/            # Auth hooks (useAuthActions)
│   ├── lib/              # Form configs, utilities
│   ├── pages/            # Page components (login, register)
│   └── services/         # Auth API calls (authService)
└── users/
    ├── components/        # User components (UserDialog)
    ├── hooks/            # User hooks (useUsers, useUserActions)
    ├── lib/              # Types, table columns
    ├── pages/            # Page components (UsersPage)
    └── services/         # User API calls (usersService)
```

**Purpose**: Each feature is independent and contains all related code.

**Rule**: Feature-specific code stays within the feature folder.

---

### `components/` - Shared Components
Reusable UI components used across multiple features.

```
components/
├── ui/                    # Shadcn UI primitives (Button, Input, Dialog, etc.)
├── form/                  # Form builder system
│   └── formBuilder/      # FormBuilder components and utilities
├── dataTable/            # Reusable data table components
├── layouts/              # Layout wrappers (MainLayout, BlankLayout)
├── common/               # Common shared components
├── AppSidebar.tsx        # Application sidebar
└── PermissionGuard.tsx   # Permission-based rendering
```

**Purpose**: Components shared across 2+ features.

---

### `lib/` - Core Utilities
Application-wide utilities and configuration.

```
lib/
├── config/
│   ├── http.ts           # Axios HTTP client setup
│   └── query.ts          # React Query configuration
├── types/                # Shared TypeScript types
└── utils.ts              # Helper functions (cn, formatters)
```

**Purpose**: Core app configuration and utilities.

---

### `hooks/` - Shared Hooks
Custom React hooks used across features.

```
hooks/
├── use-mobile.ts         # Mobile detection hook
└── index.ts              # Hook exports
```

**Purpose**: Hooks shared across 2+ features.

---

### `stores/` - Global State
Zustand stores for global state management.

```
stores/
├── auth.ts               # Authentication state
└── theme.ts              # Theme preferences
```

**Purpose**: Application-wide state management.

---

### `services/` - Global Services
Global API services and third-party integrations.

```
services/
└── firebase.ts           # Firebase configuration
```

**Purpose**: Global services not tied to a specific feature.

---

### `i18n/` - Internationalization
Translation files and i18n configuration.

```
i18n/
├── locales/              # Translation JSON files (en, ar, etc.)
├── config.ts             # i18n setup
├── types.ts              # i18n types
└── index.ts              # Exports
```

**Purpose**: Multi-language support.

---

### `public/` - Static Assets
Static files served directly.

```
public/
└── *.svg                 # SVG icons and images
```

---

## 🎯 Key Principles

### ✅ DO
- Keep feature-specific code in `features/[feature-name]/`
- Put shared components in `components/`
- Use `lib/` for core utilities only
- Co-locate related code (components, hooks, services) within features

### ❌ DON'T
- Mix feature-specific code in shared folders
- Put business logic in `components/`
- Create deep nesting (max 3-4 levels)
- Duplicate code (extract to shared if used 2+ times)

---

## 📝 Import Examples

### Importing from Features
```typescript
// ✅ Good - Import from feature
import { useAuthActions } from '@/features/auth/hooks/useAuthActions';
import { AuthCard } from '@/features/auth/components/AuthCard';

// ✅ Also good - If index.ts exists
import { useAuthActions, AuthCard } from '@/features/auth';
```

### Importing Shared Components
```typescript
// ✅ Import shared UI components
import { Button } from '@/components/ui/button';
import { FormBuilder } from '@/components/form';
```

### Importing Utilities
```typescript
// ✅ Import from lib
import { cn } from '@/lib/utils';
import { queryClient } from '@/lib/config/query';
```

---

## 🚀 Adding New Features

When adding a new feature (e.g., `products`):

1. Create feature folder: `features/products/`
2. Add subfolders as needed:
   ```
   features/products/
   ├── components/
   ├── hooks/
   ├── lib/
   ├── pages/
   └── services/
   ```
3. Create page in `app/`: `app/(main)/products/page.tsx`
4. Import page component from feature: `import { ProductsPage } from '@/features/products/pages/ProductsPage';`

---

**Last Updated**: 2026-01-03
