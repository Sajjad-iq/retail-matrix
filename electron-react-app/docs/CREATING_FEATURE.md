# Creating a Feature

Quick guide on how to create a new feature module in this Next.js project.

---

## 📁 Feature Structure

```
features/[name]/
├── components/     # Feature UI components
├── hooks/         # React Query hooks
├── lib/           # Types, schemas, utilities
├── pages/         # Page components
└── services/      # API service calls
```

---

## 🚀 Step-by-Step Guide

Use the **users feature** as a reference for creating new features.

### 1. Create Service

**Reference**: [`features/users/services/usersService.ts`](file:///home/sajjad/Documents/nextjs-starter/features/users/services/usersService.ts)

Create API service with CRUD operations:
- `getAll()` - Fetch all items
- `getById(id)` - Fetch single item
- `create(data)` - Create new item
- `update(id, data)` - Update existing item
- `delete(id)` - Delete item

---

### 2. Create Hooks

**Reference**: [`features/users/hooks/useUserActions.ts`](file:///home/sajjad/Documents/nextjs-starter/features/users/hooks/useUserActions.ts)

Create TanStack Query hooks:
- `useItems()` - Query for list
- `useItem(id)` - Query for single item
- `useCreateItem()` - Mutation for create
- `useUpdateItem()` - Mutation for update
- `useDeleteItem()` - Mutation for delete

**Key patterns**:
- Use `useQuery` for fetching data
- Use `useMutation` for create/update/delete
- Invalidate queries after mutations
- Add toast notifications for success/error

---

### 3. Create Types & Schemas

**Type Reference**: [`features/users/lib/types.ts`](file:///home/sajjad/Documents/nextjs-starter/features/users/lib/types.ts)

Define TypeScript types for your domain.

**Schema Reference**: [`features/users/components/UserDialog.tsx`](file:///home/sajjad/Documents/nextjs-starter/features/users/components/UserDialog.tsx#L19-L25)

Create Zod schema for form validation (see schema in UserDialog).

**Columns Reference**: [`features/users/lib/usersColumns.tsx`](file:///home/sajjad/Documents/nextjs-starter/features/users/lib/usersColumns.tsx)

Create table columns with:
- Column definitions
- Custom cell renderers (badges, dates, etc.)
- Actions column with dialog trigger

---

### 4. Create Components

**Dialog Reference**: [`features/users/components/UserDialog.tsx`](file:///home/sajjad/Documents/nextjs-starter/features/users/components/UserDialog.tsx)

Create dialog component with:
- `children` prop for trigger element
- `DialogTrigger` wrapping children
- FormBuilder for form
- `onSuccess` callback for refetch
- `DialogClose` ref for programmatic close

**Key pattern**:
```tsx
<Dialog>
  <DialogTrigger asChild>{children}</DialogTrigger>
  <DialogContent>
    <FormBuilder onSubmit={handleSubmit}>
      {/* fields */}
    </FormBuilder>
    <DialogClose ref={closeRef} className="hidden" />
  </DialogContent>
</Dialog>
```

---

### 5. Create Page

**Reference**: [`features/users/pages/UsersPage.tsx`](file:///home/sajjad/Documents/nextjs-starter/features/users/pages/UsersPage.tsx)

Create page component with:
- DataTable for list view
- Dialog wrapped around "Add" button
- Columns with `onSuccess` callback
- Refetch on success

---

### 6. Add Route

**Reference**: [`app/(main)/users/page.tsx`](file:///home/sajjad/Documents/nextjs-starter/app/(main)/users/page.tsx)

Create route file that imports and renders your page component.

---

## ✅ Checklist

When creating a new feature:

- [ ] Create folder in `features/[name]/`
- [ ] Copy structure from `features/users/`
- [ ] Create service (reference: `usersService.ts`)
- [ ] Create hooks (reference: `useUserActions.ts`)
- [ ] Create types (reference: `types.ts`)
- [ ] Create columns (reference: `usersColumns.tsx`)
- [ ] Create dialog (reference: `UserDialog.tsx`)
- [ ] Create page (reference: `UsersPage.tsx`)
- [ ] Add route in `app/(main)/[name]/page.tsx`
- [ ] Test CRUD operations

---

## 📚 Reference Files

**Complete feature example**: [`features/users/`](file:///home/sajjad/Documents/nextjs-starter/features/users)

**Key files to study**:
- Service: [`usersService.ts`](file:///home/sajjad/Documents/nextjs-starter/features/users/services/usersService.ts)
- Hooks: [`useUserActions.ts`](file:///home/sajjad/Documents/nextjs-starter/features/users/hooks/useUserActions.ts)
- Types: [`types.ts`](file:///home/sajjad/Documents/nextjs-starter/features/users/lib/types.ts)
- Columns: [`usersColumns.tsx`](file:///home/sajjad/Documents/nextjs-starter/features/users/lib/usersColumns.tsx)
- Dialog: [`UserDialog.tsx`](file:///home/sajjad/Documents/nextjs-starter/features/users/components/UserDialog.tsx)
- Page: [`UsersPage.tsx`](file:///home/sajjad/Documents/nextjs-starter/features/users/pages/UsersPage.tsx)
- Route: [`app/(main)/users/page.tsx`](file:///home/sajjad/Documents/nextjs-starter/app/(main)/users/page.tsx)

---

## 🎯 Best Practices

1. **Follow the users feature pattern** - It's the reference implementation
2. **Use DialogTrigger** - No useState for dialog open/close
3. **Invalidate queries** - Refresh data after mutations
4. **Toast notifications** - Show success/error messages
5. **onSuccess callback** - Pass to columns and dialogs for refetch
6. **Zod validation** - Define schemas for forms
7. **TypeScript types** - Define domain types
