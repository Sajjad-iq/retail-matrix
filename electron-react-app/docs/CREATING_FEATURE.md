# Creating a Feature

Quick guide on how to create a new feature module in this Electron-React project.

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

Use the **auth feature** or **users feature** as a reference.

### 1. Create Service

**Reference**: [`features/auth/services/authService.ts`](file:///home/sajjad/Documents/retail-matrix/electron-react-app/app/features/auth/services/authService.ts)

Create API service with axios operations.
**Important**: Use raw axios calls or your http service. The global interceptor handles errors.

```typescript
export const myService = {
    getAll: async () => {
        const response = await httpService.getAxiosInstance().get('/api/items');
        return response.data;
    }
}
```

---

### 2. Create Hooks

**Reference**: [`features/auth/hooks/useAuthActions.ts`](file:///home/sajjad/Documents/retail-matrix/electron-react-app/app/features/auth/hooks/useAuthActions.ts)

Create TanStack Query hooks.

**Key patterns**:
- Use `useQuery` for fetching.
- Use `useMutation` for actions.
- **DO NOT** add `onError` handlers for API errors using `toast.error`. The global interceptor in `lib/config/http.ts` handles this automatically.
- Only add `toast.success` in `onSuccess`.

```typescript
export function useCreateItem() {
    const queryClient = useQueryClient();
    
    return useMutation({
        mutationFn: (data) => myService.create(data),
        onSuccess: () => {
            toast.success('Created successfully');
            queryClient.invalidateQueries({ queryKey: ['items'] });
        },
        // No onError needed for standard API errors!
    });
}
```

---

### 3. Create Types & Schemas

Define TypeScript interfaces in `lib/types.ts`.
Define Zod schemas for forms.

---

### 4. Create Components

Create your UI components.

---

### 5. Create Page

Create page components in `features/[name]/pages/`.

---

### 6. Add Route

**Reference**: [`app/routes.tsx`](file:///home/sajjad/Documents/retail-matrix/electron-react-app/app/routes.tsx)

Register your new page in the router configuration.

```typescript
// app/routes.tsx
{
    path: '/my-feature',
    element: <MyFeaturePage />,
}
```

---

## ✅ Checklist

- [ ] Create folder in `features/[name]/`
- [ ] Create service 
- [ ] Create hooks (Remember: No manual error handling for API calls)
- [ ] Create types
- [ ] Create page component
- [ ] Register route in `app/routes.tsx`

---

## 🎯 Best Practices

1. **Centralized Error Handling**: Trust the `http.ts` interceptor. Don't catch errors in hooks just to show a toast.
2. **Invalidate queries**: Always refresh data after mutations.
3. **Zod validation**: Validate forms before submission.
