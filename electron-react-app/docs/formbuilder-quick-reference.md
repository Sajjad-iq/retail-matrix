# FormBuilder Quick Reference

> **📖 For validation patterns and Zod schemas, see [validation-patterns.mdc](./validation-patterns.mdc)**

## Minimal Working Example

```typescript
'use client';

import { FormBuilder } from '@/components/form';
import { mySchema, type MyFormValues } from './validations';
import { useMutation } from './hooks';

export function MyForm() {
  const mutation = useMutation();
  
  return (
    <FormBuilder
      onSubmit={(values: MyFormValues) => mutation.mutate(values)}
      schema={mySchema}
      defaultValues={{ field: '' }}
      loading={mutation.isPending}
      className="space-y-4"
    >
      <FormBuilder.Text name="field" label="Label" required />
      <FormBuilder.Submit loadingText="Submitting...">Submit</FormBuilder.Submit>
    </FormBuilder>
  );
}
```

## Available Fields

| Component | Usage |
|-----------|-------|
| `FormBuilder.Text` | Text input |
| `FormBuilder.Email` | Email input with validation |
| `FormBuilder.Password` | Password input (hidden) |
| `FormBuilder.Textarea` | Multi-line text |
| `FormBuilder.Number` | Numeric input |
| `FormBuilder.Select` | Dropdown select |
| `FormBuilder.Phone` | Phone number input |
| `FormBuilder.Checkbox` | Boolean checkbox |
| `FormBuilder.Switch` | Toggle switch |
| `FormBuilder.Radio` | Radio button group |
| `FormBuilder.Date` | Date picker |
| `FormBuilder.Image` | Image upload |
| `FormBuilder.Submit` | Submit button |

## Common Props

```typescript
// All field components accept:
name: string              // Required - must match schema
label?: string           // Field label
placeholder?: string     // Placeholder text
required?: boolean       // Shows asterisk
disabled?: boolean       // Disables field
className?: string       // Custom classes
onChange?: (e) => void   // Custom change handler
icon?: LucideIcon        // Icon component

// FormBuilder root accepts:
onSubmit: (values) => void     // Submit handler
schema?: ZodSchema             // Validation schema
defaultValues?: Record<...>    // Initial values
loading?: boolean              // Loading state
className?: string             // Container classes
```

## Validation Schema

> **📖 For complete validation patterns and Zod schemas, see [validation-patterns.mdc](./validation-patterns.mdc)**

```typescript
// validations.ts - Always define schemas in separate files
import * as z from 'zod';
import { ARABIC_TEXT_REGEX } from '@/lib/validations/regex'; // Use shared regex

export const myFormSchema = z.object({
  name: z.string().min(1, 'Name required'),
  email: z.string().email('Invalid email'),
  // See validation-patterns.mdc for more patterns
});

export type MyFormValues = z.infer<typeof myFormSchema>;
```

## Conditional Fields Pattern

```typescript
'use client';

import { useFormContext } from 'react-hook-form';

function FormContent() {
  const { watch } = useFormContext<MyFormValues>();
  const showField = watch('checkbox');
  
  return (
    <>
      <FormBuilder.Checkbox name="checkbox" label="Show field" />
      {showField && <FormBuilder.Text name="conditional" label="Conditional" />}
      <FormBuilder.Submit>Submit</FormBuilder.Submit>
    </>
  );
}

export function MyForm() {
  return (
    <FormBuilder onSubmit={handleSubmit} schema={schema} defaultValues={{}}>
      <FormContent />
    </FormBuilder>
  );
}
```

## Custom Input Formatting

```typescript
<FormBuilder.Text
  name="phone"
  label="Phone"
  onChange={(e) => {
    // Only digits, max 11
    e.target.value = e.target.value.replace(/[^0-9]/g, '').slice(0, 11);
  }}
/>
```

## Multi-Step Form Pattern

```typescript
const [step, setStep] = useState(1);
const [data, setData] = useState({});

const handleStep1 = (values) => {
  setData({ ...data, ...values });
  setStep(2);
};

return (
  <>
    {step === 1 && (
      <FormBuilder onSubmit={handleStep1} schema={step1Schema} defaultValues={{}}>
        {/* Step 1 fields */}
      </FormBuilder>
    )}
    {step === 2 && (
      <FormBuilder onSubmit={finalSubmit} schema={step2Schema} defaultValues={{}}>
        {/* Step 2 fields */}
        <Button type="button" onClick={() => setStep(1)}>Back</Button>
      </FormBuilder>
    )}
  </>
);
```

## Checklist

- [ ] Added `'use client'` directive
- [ ] Created Zod schema in separate file
- [ ] Exported TypeScript type from schema
- [ ] Passed `loading` state to FormBuilder
- [ ] Added `loadingText` to Submit button
- [ ] Used typed submit handler
- [ ] Set appropriate `defaultValues`
- [ ] Applied responsive Tailwind classes
- [ ] Validated field names match schema keys

## Common Field Examples

```typescript
// Email
<FormBuilder.Email name="email" label="Email" placeholder="you@example.com" required />

// Password with confirmation
<FormBuilder.Password name="password" label="Password" required />
<FormBuilder.Password name="confirmPassword" label="Confirm" required />

// Select dropdown
<FormBuilder.Select 
  name="country" 
  label="Country"
  options={[
    { label: 'USA', value: 'us' },
    { label: 'UK', value: 'uk' },
  ]}
  required 
/>

// Phone with formatting
<FormBuilder.Text
  name="phone"
  label="Phone"
  placeholder="07XXXXXXXXX"
  className="text-center tracking-wider"
  onChange={(e) => {
    e.target.value = e.target.value.replace(/[^0-9]/g, '').slice(0, 11);
  }}
  required
/>

// Checkbox
<FormBuilder.Checkbox name="agree" label="I agree to terms" />

// Date picker
<FormBuilder.Date 
  name="birthDate" 
  label="Birth Date"
  maxDate={new Date()}
/>

// Number with constraints
<FormBuilder.Number 
  name="age" 
  label="Age"
  min={0}
  max={120}
  step={1}
/>
```

## Submit Button Variations

```typescript
// Basic
<FormBuilder.Submit loadingText="Submitting...">Submit</FormBuilder.Submit>

// Full width with custom classes
<FormBuilder.Submit 
  loadingText="Processing..." 
  className="w-full h-11"
>
  Create Account
</FormBuilder.Submit>

// In a button group
<div className="flex gap-3">
  <Button type="button" variant="outline" className="flex-1">Cancel</Button>
  <FormBuilder.Submit loadingText="Saving..." className="flex-1">
    Save
  </FormBuilder.Submit>
</div>
```

## Validation Patterns

> **📖 For all validation patterns, Zod schemas, regex patterns, and error messages, see [validation-patterns.mdc](./validation-patterns.mdc)**

This includes:
- Shared regex patterns (phone, email, Arabic/English text, etc.)
- Complete Zod schema examples
- onChange character filtering
- Error message best practices
