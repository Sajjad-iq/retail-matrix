# Dialog Patterns

## Overview
This document covers dialog (modal) patterns using shadcn/ui Dialog component with FormBuilder for form-based dialogs.

> **📖 Related Documentation:**
> - [FormBuilder Pattern](./formbuilder.mdc) - Form implementation
> - [Validation Patterns](./validation-patterns.mdc) - Schema validation

---

## Core Principles

1. **Controlled Dialogs**: Use `open` and `onOpenChange` props for state management
2. **FormBuilder Integration**: Use FormBuilder for all form dialogs
3. **Separate Components**: Create dedicated dialog components, not inline
4. **RTL Support**: Add `dir="rtl"` for Arabic content
5. **Type Safety**: Define proper TypeScript interfaces for props

---

## Basic Dialog Pattern

### Dialog Component Structure

```typescript
// components/FeatureDialog.tsx
'use client';

import { Dialog, DialogContent, DialogHeader, DialogTitle } from '@/components/ui/dialog';
import { FormBuilder } from '@/components/form';
import { featureSchema, type FeatureFormValues } from '../validations';

interface FeatureDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onSave: (values: FeatureFormValues) => void;
  initialValues: FeatureFormValues;
}

export function FeatureDialog({ 
  open, 
  onOpenChange, 
  onSave, 
  initialValues 
}: FeatureDialogProps) {
  const handleSubmit = (values: FeatureFormValues) => {
    onSave(values);
    onOpenChange(false);
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent dir="rtl" className="sm:max-w-[500px]">
        <DialogHeader>
          <DialogTitle>إضافة عنصر جديد</DialogTitle>
        </DialogHeader>

        <FormBuilder
          onSubmit={handleSubmit}
          schema={featureSchema}
          defaultValues={initialValues}
          className="space-y-4"
        >
          <FormBuilder.Text
            name="nameArabic"
            label="الاسم بالعربية *"
            required
            dir="rtl"
            onChange={(e) => {
              const filtered = e.target.value
                .split('')
                .filter((char) => /[\u0600-\u06FF\s-]/.test(char))
                .join('');
              e.target.value = filtered;
            }}
          />

          <FormBuilder.Text
            name="name"
            label="Name *"
            required
            dir="ltr"
            onChange={(e) => {
              const filtered = e.target.value
                .split('')
                .filter((char) => /[a-zA-Z\s-]/.test(char))
                .join('');
              e.target.value = filtered;
            }}
          />

          <FormBuilder.Checkbox name="isActive" label="نشط" />

          <FormBuilder.Submit>حفظ</FormBuilder.Submit>
        </FormBuilder>
      </DialogContent>
    </Dialog>
  );
}
```

### Usage in Parent Component

```typescript
// page.tsx
'use client';

import { useState } from 'react';
import { FeatureDialog } from './components/FeatureDialog';

export default function FeaturePage() {
  const [dialogOpen, setDialogOpen] = useState(false);
  const [formData, setFormData] = useState(initialValues);

  const handleSave = (values: FeatureFormValues) => {
    // Handle save logic
    console.log('Saved:', values);
  };

  return (
    <div>
      <Button onClick={() => setDialogOpen(true)}>
        إضافة عنصر
      </Button>

      <FeatureDialog
        open={dialogOpen}
        onOpenChange={setDialogOpen}
        onSave={handleSave}
        initialValues={formData}
      />
    </div>
  );
}
```

---

## Dialog Sizes

Use appropriate sizes for different content types:

```typescript
// Small dialog - Simple forms
<DialogContent className="sm:max-w-[400px]">

// Medium dialog - Standard forms (default)
<DialogContent className="sm:max-w-[500px]">

// Large dialog - Complex forms
<DialogContent className="sm:max-w-[600px]">

// Extra large dialog - Multi-section forms
<DialogContent className="sm:max-w-[800px]">
```

---

## Dialog with Description

Add a description for more context:

```typescript
import { 
  Dialog, 
  DialogContent, 
  DialogHeader, 
  DialogTitle,
  DialogDescription 
} from '@/components/ui/dialog';

<Dialog open={open} onOpenChange={onOpenChange}>
  <DialogContent dir="rtl" className="sm:max-w-[500px]">
    <DialogHeader className="text-right">
      <DialogTitle>إضافة عنصر جديد</DialogTitle>
      <DialogDescription>
        قم بملء جميع الحقول المطلوبة لإضافة عنصر جديد
      </DialogDescription>
    </DialogHeader>
    {/* Form content */}
  </DialogContent>
</Dialog>
```

---

## Edit vs Create Mode

Handle both create and edit in a single dialog:

```typescript
interface ItemDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  item?: Item; // Optional for edit mode
  onSave: (values: ItemFormValues) => void;
}

export function ItemDialog({ open, onOpenChange, item, onSave }: ItemDialogProps) {
  const isEditMode = !!item;

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent dir="rtl" className="sm:max-w-[500px]">
        <DialogHeader>
          <DialogTitle>
            {isEditMode ? 'تعديل العنصر' : 'إضافة عنصر جديد'}
          </DialogTitle>
        </DialogHeader>

        <FormBuilder
          onSubmit={onSave}
          schema={itemSchema}
          defaultValues={item || initialValues}
          className="space-y-4"
        >
          {/* Form fields */}
          <FormBuilder.Submit>
            {isEditMode ? 'حفظ التعديلات' : 'إضافة'}
          </FormBuilder.Submit>
        </FormBuilder>
      </DialogContent>
    </Dialog>
  );
}
```

---

## Custom Footer with Actions

Add custom footer buttons:

```typescript
import { DialogFooter } from '@/components/ui/dialog';

<FormBuilder
  onSubmit={handleSubmit}
  schema={schema}
  defaultValues={values}
  className="space-y-4"
>
  <FormBuilder.Text name="field" label="Label" />

  <DialogFooter className="flex-row-reverse gap-2">
    <Button
      type="button"
      variant="outline"
      onClick={() => onOpenChange(false)}
    >
      إلغاء
    </Button>
    <FormBuilder.Submit loadingText="جاري الحفظ...">
      حفظ
    </FormBuilder.Submit>
  </DialogFooter>
</FormBuilder>
```

---

## Confirmation Dialog

For simple confirmations without forms:

```typescript
export function ConfirmDialog({ 
  open, 
  onOpenChange, 
  title, 
  description,
  onConfirm 
}: ConfirmDialogProps) {
  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent dir="rtl" className="sm:max-w-[400px]">
        <DialogHeader>
          <DialogTitle>{title}</DialogTitle>
          <DialogDescription>{description}</DialogDescription>
        </DialogHeader>

        <DialogFooter className="flex-row-reverse gap-2">
          <Button
            variant="outline"
            onClick={() => onOpenChange(false)}
          >
            إلغاء
          </Button>
          <Button
            variant="destructive"
            onClick={() => {
              onConfirm();
              onOpenChange(false);
            }}
          >
            تأكيد
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
```

---

## Best Practices

### ✅ DO

1. **Separate Dialog Components**: Create dedicated files for dialogs
   ```typescript
   // components/UserDialog.tsx
   export function UserDialog({ ... }) { }
   ```

2. **Use FormBuilder**: Always use FormBuilder for form dialogs
   ```typescript
   <FormBuilder onSubmit={handleSubmit} schema={schema}>
   ```

3. **Controlled State**: Manage dialog state in parent component
   ```typescript
   const [open, setOpen] = useState(false);
   ```

4. **RTL Support**: Add `dir="rtl"` for Arabic content
   ```typescript
   <DialogContent dir="rtl">
   ```

5. **Close on Submit**: Close dialog after successful submission
   ```typescript
   const handleSubmit = (values) => {
     onSave(values);
     onOpenChange(false);
   };
   ```

6. **Type Safety**: Define proper interfaces for props
   ```typescript
   interface DialogProps {
     open: boolean;
     onOpenChange: (open: boolean) => void;
   }
   ```

### ❌ DON'T

1. **Don't Create Inline Dialogs**: Avoid inline dialog JSX in pages
   ```typescript
   // ❌ Bad
   <Dialog>
     <DialogContent>
       {/* Inline form */}
     </DialogContent>
   </Dialog>
   ```

2. **Don't Skip Validation**: Always use Zod schema
   ```typescript
   // ❌ Bad
   <FormBuilder onSubmit={...} /> // No schema
   ```

3. **Don't Mix Languages**: Use consistent RTL/LTR
   ```typescript
   // ❌ Bad - missing dir
   <DialogContent>
     <DialogTitle>عنوان عربي</DialogTitle>
   </DialogContent>
   ```

4. **Don't Skip Loading States**: Use loading prop
   ```typescript
   // ✅ Good
   <FormBuilder loading={mutation.isPending}>
   ```

---

## Integration with DataTable

Common pattern for table actions:

```typescript
// config/table-config.tsx
export const createColumns = (options: ColumnsOptions) => [
  // ... other columns
  {
    id: 'actions',
    header: 'الإجراءات',
    cell: ({ row }) => (
      <div className="flex justify-center gap-2">
        <Button
          variant="ghost"
          size="sm"
          onClick={() => options.onEdit?.(row.original)}
        >
          <Pencil className="h-4 w-4" />
        </Button>
      </div>
    ),
  },
];

// page.tsx
export default function Page() {
  const [editDialog, setEditDialog] = useState(false);
  const [selectedItem, setSelectedItem] = useState<Item | null>(null);

  const handleEdit = (item: Item) => {
    setSelectedItem(item);
    setEditDialog(true);
  };

  return (
    <>
      <DataTable
        data={data}
        columns={createColumns({ onEdit: handleEdit })}
      />

      <ItemDialog
        open={editDialog}
        onOpenChange={setEditDialog}
        item={selectedItem}
        onSave={handleSave}
      />
    </>
  );
}
```

---

## Summary

- Always use separate dialog components
- Integrate FormBuilder for form dialogs
- Use controlled state (open/onOpenChange)
- Support RTL for Arabic content
- Define proper TypeScript interfaces
- Close dialog on successful submission

**For validation patterns, see [validation-patterns.mdc](./validation-patterns.mdc)**  
**For form patterns, see [formbuilder.mdc](./formbuilder.mdc)**
