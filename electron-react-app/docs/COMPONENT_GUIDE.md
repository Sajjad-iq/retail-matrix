# Component Guide

Simple guide explaining UI vs Shared components and how to create them.

---

## 🎯 The Difference

**UI Components** (`components/ui/`) = Just styling, no logic  
**Shared Components** (`components/`) = Styling + logic

---

## 📦 UI Components

### What They Are
Styled primitives from Shadcn UI - buttons, inputs, dialogs.

### Rules
- ✅ Only styling (colors, sizes, variants)
- ❌ No API calls
- ❌ No business logic
- ❌ No data fetching

### Examples

**Button**
```tsx
<Button variant="destructive">Delete</Button>
<Button variant="outline" size="sm">Cancel</Button>
```

**Input**
```tsx
<Input type="email" placeholder="Enter email" />
```

**Dialog**
```tsx
<Dialog open={isOpen} onOpenChange={setIsOpen}>
  <DialogContent>
    <DialogTitle>Edit User</DialogTitle>
    {/* Your content */}
  </DialogContent>
</Dialog>
```

**Others**: `badge`, `card`, `table`, `select`, `checkbox`, `switch`

---

## 🔧 Shared Components

### What They Are
Components that combine UI + logic for reusable features.

### Rules
- ✅ Use UI components
- ✅ Add logic (sorting, validation, etc.)
- ✅ Manage state
- ✅ Reusable across features

---

### DataTable

**What**: Table with sorting, pagination, filtering

**Uses**: `Table`, `Button`, `Input`, `Select`

```tsx
<DataTable
  data={users}
  columns={userColumns}
  pagination={{ page: 1, size: 20, totalElements: 100 }}
  onPageChange={handlePageChange}
/>
```

**Why shared**: Has table logic, manages state, integrates TanStack Table

---

### FormBuilder

**What**: Form system with validation

**Uses**: `Input`, `Select`, `Checkbox`, `Button`

```tsx
<FormBuilder onSubmit={handleSubmit} schema={userSchema}>
  <FormBuilder.Text name="name" label="Name" required />
  <FormBuilder.Email name="email" label="Email" />
  <FormBuilder.Submit>Save</FormBuilder.Submit>
</FormBuilder>
```

**Why shared**: Handles validation, form state, react-hook-form integration

---

### Common Components

**EmptyState** - Standardized empty state
```tsx
<EmptyState
  icon={Inbox}
  title="No users found"
  action={{ label: "Add User", onClick: handleAdd }}
/>
```

**ErrorState** - Error display with retry
```tsx
<ErrorState error="Failed to load" onRetry={refetch} />
```

**Spinner** - Loading indicator
```tsx
<Spinner size="lg" />
```

---

## 🤔 Quick Decision

**Need a styled button?** → `<Button variant="destructive">`  
**Need a table with pagination?** → `<DataTable>`  
**Need a form with validation?** → `<FormBuilder>`  
**Need feature-specific?** → `features/[name]/components/`

---

## 🛠️ How to Create

### UI Component

**File**: `components/ui/badge.tsx`

```tsx
import { cva } from "class-variance-authority"
import { cn } from "@/lib/utils"

const badgeVariants = cva(
  "inline-flex items-center rounded-full px-2.5 py-0.5 text-xs",
  {
    variants: {
      variant: {
        default: "bg-primary text-primary-foreground",
        destructive: "bg-destructive text-destructive-foreground",
      },
    },
  }
)

export function Badge({ variant, className, ...props }) {
  return <div className={cn(badgeVariants({ variant }), className)} {...props} />
}
```

---

### Shared Component

**File**: `components/common/SearchInput.tsx`

```tsx
import { useState } from 'react'
import { Search } from 'lucide-react'
import { Input } from '@/components/ui/input'
import { Button } from '@/components/ui/button'

export function SearchInput({ onSearch, placeholder = "Search..." }) {
  const [query, setQuery] = useState('')

  return (
    <form onSubmit={(e) => { e.preventDefault(); onSearch(query) }}>
      <Input value={query} onChange={(e) => setQuery(e.target.value)} />
      <Button type="submit"><Search /></Button>
    </form>
  )
}
```

**Export**: Add to `components/common/index.ts`
```tsx
export { SearchInput } from './SearchInput'
```

---

### Feature Component

**File**: `features/users/components/UserDialog.tsx`

```tsx
import { Dialog, DialogContent, DialogTitle } from '@/components/ui/dialog'
import { FormBuilder } from '@/components/form'
import { useUser, useUpdateUser } from '../hooks/useUsers'

export function UserDialog({ userId, open, onClose }) {
  const { data: user } = useUser(userId)
  const updateUser = useUpdateUser()

  return (
    <Dialog open={open} onOpenChange={onClose}>
      <DialogContent>
        <DialogTitle>{userId ? 'Edit' : 'Add'} User</DialogTitle>
        <FormBuilder
          onSubmit={(data) => updateUser.mutateAsync(data)}
          defaultValues={user}
        >
          <FormBuilder.Text name="name" label="Name" />
          <FormBuilder.Email name="email" label="Email" />
          <FormBuilder.Submit>Save</FormBuilder.Submit>
        </FormBuilder>
      </DialogContent>
    </Dialog>
  )
}
```

---

## ✅ Checklist

Before creating a component:

- [ ] Does it exist? Check `components/ui/` and `components/`
- [ ] UI primitive? → `components/ui/`
- [ ] Reused across features? → `components/[category]/`
- [ ] Feature-specific? → `features/[feature]/components/`

---

## 📋 Summary

| Type | Location | Logic | Example |
|------|----------|-------|---------|
| UI | `components/ui/` | ❌ | Button, Input |
| Shared | `components/dataTable/`, `components/form/` | ✅ | DataTable, FormBuilder |
| Feature | `features/[name]/components/` | ✅ | UserDialog |

**Think of it**:
- `Button` = Lego piece
- `FormBuilder` = Lego set
- `UserDialog` = Your creation

---

## 📚 Resources

- [Shadcn UI](https://ui.shadcn.com) - Component examples
- [CVA](https://cva.style/docs) - Variants
- [React TypeScript](https://react-typescript-cheatsheet.netlify.app) - Types

