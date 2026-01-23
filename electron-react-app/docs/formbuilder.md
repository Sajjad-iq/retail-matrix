# FormBuilder Pattern Rules

## Overview
FormBuilder is a custom form abstraction built on top of `react-hook-form` and `zod` for validation. It provides a declarative, type-safe way to create forms with consistent styling and validation.

> **📖 For comprehensive validation patterns and Zod schemas, see [validation-patterns.mdc](./validation-patterns.mdc)**

## Core Principles

1. **Zod Schema First**: Always define Zod schemas for validation
2. **Type Safety**: Export and use TypeScript types from Zod schemas
3. **Declarative Syntax**: Use FormBuilder compound components
4. **Client Components Only**: FormBuilder forms must use `'use client'` directive
5. **Loading States**: Pass `loading` prop to FormBuilder for async operations

## Basic Form Structure

### Simple Form Pattern

```typescript
'use client';

import { FormBuilder } from '@/components/form';
import { myFormSchema, type MyFormValues } from './validations';
import { useMutation } from './hooks';

export function MyForm() {
  const mutation = useMutation();
  
  const handleSubmit = (values: MyFormValues) => {
    mutation.mutate(values);
  };
  
  return (
    <FormBuilder
      onSubmit={handleSubmit}
      schema={myFormSchema}
      defaultValues={{ field1: '', field2: '' }}
      loading={mutation.isPending}
      className="space-y-3 sm:space-y-4"
    >
      <FormBuilder.Text
        name="field1"
        label="Field Label"
        placeholder="Enter value"
        required
      />
      
      <FormBuilder.Password
        name="field2"
        label="Password"
        placeholder="••••••••"
        required
      />
      
      <FormBuilder.Submit loadingText="Submitting...">
        Submit
      </FormBuilder.Submit>
    </FormBuilder>
  );
}
```

## Validation with Zod Schemas

> **📖 For complete validation patterns, Zod schemas, regex patterns, and onChange filtering, see [validation-patterns.mdc](./validation-patterns.mdc)**

### Key Validation Principles

1. **Always define Zod schemas in separate `validations.ts` files**
2. **Use shared regex from `@/lib/validations/regex`** - never define inline patterns
3. **Export types using `z.infer<typeof schema>`** for type safety
4. **Use `onChange` filtering for character-level validation** (see validation-patterns.mdc)

```typescript
// validations.ts - Define schemas separately
import * as z from 'zod';
import { ARABIC_TEXT_REGEX, ENGLISH_TEXT_REGEX } from '@/lib/validations/regex';

export const myFormSchema = z.object({
  nameArabic: z
    .string()
    .min(1, 'الاسم بالعربية مطلوب')
    .regex(ARABIC_TEXT_REGEX, 'يجب إدخال أحرف عربية فقط'),
  name: z
    .string()
    .min(1, 'Name is required')
    .regex(ENGLISH_TEXT_REGEX, 'English letters only'),
});

// Export the inferred type
export type MyFormValues = z.infer<typeof myFormSchema>;
```

**For all validation patterns including:**
- Phone number validation
- Email/Phone union validation
- Password requirements
- Arabic/English text validation
- Optional fields
- Conditional validation
- Custom superRefine patterns

**👉 See [validation-patterns.mdc](./validation-patterns.mdc)**

## FormBuilder API

### Root Component Props

```typescript
<FormBuilder
  onSubmit={(values) => void}     // Required: Form submit handler
  schema={zodSchema}               // Optional: Zod schema for validation
  defaultValues={{}}               // Optional: Initial form values
  loading={boolean}                // Optional: Loading state (disables fields)
  className="space-y-4"            // Optional: Form container classes
>
  {/* Field components */}
</FormBuilder>
```

### Available Field Components

#### Text Input
```typescript
<FormBuilder.Text
  name="fieldName"              // Required: Must match schema key
  label="Field Label"           // Optional: Label text
  placeholder="Placeholder"     // Optional
  required                      // Optional: Shows asterisk
  disabled                      // Optional
  className="custom-classes"    // Optional
  onChange={(e) => {}}          // Optional: Custom change handler
  icon={LucideIcon}             // Optional: Icon component
/>
```

#### Email Input
```typescript
<FormBuilder.Email
  name="email"
  label="Email Address"
  placeholder="you@example.com"
  required
/>
```

#### Password Input
```typescript
<FormBuilder.Password
  name="password"
  label="Password"
  placeholder="••••••••"
  required
/>
```

#### Textarea
```typescript
<FormBuilder.Textarea
  name="description"
  label="Description"
  placeholder="Enter description"
  rows={4}
/>
```

#### Number Input
```typescript
<FormBuilder.Number
  name="age"
  label="Age"
  min={0}
  max={120}
  step={1}
/>
```

#### Select Dropdown
```typescript
<FormBuilder.Select
  name="country"
  label="Country"
  options={[
    { label: 'USA', value: 'us' },
    { label: 'UK', value: 'uk' },
  ]}
  required
/>
```

#### Phone Input
```typescript
<FormBuilder.Phone
  name="phoneNumber"
  label="Phone Number"
  defaultCountry="IQ"
  required
/>
```

#### Checkbox
```typescript
<FormBuilder.Checkbox
  name="hasNationalId"
  label="I have a National ID"
/>
```

#### Switch
```typescript
<FormBuilder.Switch
  name="isActive"
  label="Active Status"
/>
```

#### Radio Group
```typescript
<FormBuilder.Radio
  name="gender"
  label="Gender"
  options={[
    { label: 'Male', value: 'male' },
    { label: 'Female', value: 'female' },
  ]}
/>
```

#### Date Picker
```typescript
<FormBuilder.Date
  name="birthDate"
  label="Birth Date"
  minDate={new Date(1900, 0, 1)}
  maxDate={new Date()}
  dateFormat="yyyy-MM-dd"
/>
```

#### Image Upload
```typescript
<FormBuilder.Image
  name="profilePhoto"
  label="Profile Photo"
  organizationId="org-123"
  mediaCategory="profile"
  maxSize={5 * 1024 * 1024}  // 5MB
  previewAlt="Profile photo"
/>
```

#### Submit Button
```typescript
<FormBuilder.Submit 
  loadingText="Submitting..."    // Text shown during loading
  className="w-full"             // Optional classes
>
  Submit Form
</FormBuilder.Submit>
```

## Advanced Patterns

### Accessing Form Context
Use `useFormContext` to access form state and methods inside FormBuilder children:

```typescript
'use client';

import { useFormContext } from 'react-hook-form';
import { FormBuilder } from '@/components/form';

function FormContent() {
  const { watch } = useFormContext<MyFormValues>();
  const hasNationalId = watch('hasNationalId');
  
  return (
    <>
      <FormBuilder.Checkbox 
        name="hasNationalId" 
        label="Has National ID" 
      />
      
      {hasNationalId ? (
        <FormBuilder.Text
          name="nationalId"
          label="National ID"
          required
        />
      ) : (
        <FormBuilder.Text
          name="civilId"
          label="Civil ID"
          required
        />
      )}
      
      <FormBuilder.Submit>Submit</FormBuilder.Submit>
    </>
  );
}

export function MyForm() {
  return (
    <FormBuilder
      onSubmit={handleSubmit}
      schema={mySchema}
      defaultValues={{...}}
    >
      <FormContent />
    </FormBuilder>
  );
}
```

### Custom onChange Handlers for Character Filtering
Filter out invalid characters as the user types using character-by-character filtering.

**Key Points**:
- Use `.split('').filter().join('')` pattern for character filtering
- Don't use `e.preventDefault()` - it doesn't work for text input
- Use **inline character patterns** (e.g., `/[a-zA-Z\s-]/`) not shared regex
- Character patterns test single chars; shared regex patterns test full strings

```typescript
// Arabic text input - filter out non-Arabic characters
<FormBuilder.Text
  name="nameArabic"
  label="الاسم بالعربية"
  dir="rtl"
  onChange={(e) => {
    // Filter out any non-Arabic characters
    const filtered = e.target.value
      .split('')
      .filter((char) => /[\u0600-\u06FF\s-]/.test(char))  // Character pattern
      .join('');
    e.target.value = filtered;
  }}
/>

// English text input - filter out non-English characters
<FormBuilder.Text
  name="name"
  label="Name"
  dir="ltr"
  onChange={(e) => {
    // Filter out any non-English characters
    const filtered = e.target.value
      .split('')
      .filter((char) => /[a-zA-Z\s-]/.test(char))  // Character pattern
      .join('');
    e.target.value = filtered;
  }}
/>

// Phone number - strip non-digits and limit length
<FormBuilder.Text
  name="phoneNumber"
  label="Phone Number"
  onChange={(e) => {
    // Strip non-digits and limit length
    const filtered = e.target.value.replace(/[^0-9]/g, '').slice(0, 11);
    e.target.value = filtered;
  }}
/>

// National ID - only digits, limit to 12 characters
<FormBuilder.Text
  name="nationalId"
  label="الرقم الوطني"
  onChange={(e) => {
    const filtered = e.target.value.replace(/[^0-9]/g, '').slice(0, 12);
    e.target.value = filtered;
  }}
/>
```

**Why inline character patterns?**
- Shared regex like `ARABIC_TEXT_REGEX = /^[\u0600-\u06FF\s-]+$/` are for validating **entire strings** (anchored with `^...$`)
- Character filtering needs simple patterns like `/[\u0600-\u06FF\s-]/` to test **single characters**
- Use shared regex in Zod schemas, inline patterns in onChange handlers

### Multi-Step Form Pattern

```typescript
'use client';

import { useState } from 'react';

export function MultiStepForm() {
  const [step, setStep] = useState(1);
  const [data, setData] = useState({});
  
  const handleStep1 = (values: Step1Values) => {
    setData({ ...data, ...values });
    setStep(2);
  };
  
  const handleStep2 = (values: Step2Values) => {
    const finalData = { ...data, ...values };
    // Submit final data
  };
  
  return (
    <>
      {step === 1 && (
        <FormBuilder
          onSubmit={handleStep1}
          schema={step1Schema}
          defaultValues={{...}}
        >
          {/* Step 1 fields */}
        </FormBuilder>
      )}
      
      {step === 2 && (
        <FormBuilder
          onSubmit={handleStep2}
          schema={step2Schema}
          defaultValues={{...}}
        >
          {/* Step 2 fields */}
          <Button type="button" onClick={() => setStep(1)}>
            Back
          </Button>
        </FormBuilder>
      )}
    </>
  );
}
```

### Dynamic Field Display with Watch

```typescript
function FormFields() {
  const { watch } = useFormContext<FormValues>();
  const nationalId = watch('nationalId');
  
  return (
    <>
      <FormBuilder.Text
        name="nationalId"
        label="National ID (12 digits)"
        onChange={(e) => {
          e.target.value = e.target.value.replace(/[^0-9]/g, '').slice(0, 12);
        }}
      />
      
      {nationalId && (
        <p className="text-muted-foreground text-xs text-center">
          {nationalId.length}/12 digits
        </p>
      )}
    </>
  );
}
```

## Integration Patterns

### With React Query / TanStack Query

```typescript
'use client';

import { useMutation } from '@tanstack/react-query';
import { FormBuilder } from '@/components/form';

export function LoginForm() {
  const loginMutation = useMutation({
    mutationFn: (values: LoginFormValues) => 
      fetch('/api/login', {
        method: 'POST',
        body: JSON.stringify(values),
      }),
  });
  
  const handleSubmit = (values: LoginFormValues) => {
    loginMutation.mutate(values);
  };
  
  return (
    <FormBuilder
      onSubmit={handleSubmit}
      schema={loginFormSchema}
      defaultValues={{ email: '', password: '' }}
      loading={loginMutation.isPending}
    >
      <FormBuilder.Email name="email" label="Email" required />
      <FormBuilder.Password name="password" label="Password" required />
      <FormBuilder.Submit loadingText="Logging in...">
        Login
      </FormBuilder.Submit>
    </FormBuilder>
  );
}
```

### With Custom Hooks

```typescript
// hooks/useLogin.ts
export function useLogin() {
  return useMutation({
    mutationFn: async (values: LoginFormValues) => {
      const response = await fetch('/api/login', {
        method: 'POST',
        body: JSON.stringify(values),
      });
      if (!response.ok) throw new Error('Login failed');
      return response.json();
    },
  });
}

// components/LoginForm.tsx
'use client';

import { useLogin } from '@/hooks/useLogin';

export function LoginForm() {
  const loginMutation = useLogin();
  
  return (
    <FormBuilder
      onSubmit={(values) => loginMutation.mutate(values)}
      schema={loginFormSchema}
      defaultValues={{ emailOrPhone: '', password: '' }}
      loading={loginMutation.isPending}
    >
      {/* Fields */}
    </FormBuilder>
  );
}
```

## Styling Guidelines

### Responsive Spacing
```typescript
<FormBuilder
  className="space-y-3 sm:space-y-4"  // Mobile-first responsive
>
```

### Button Styling
```typescript
<FormBuilder.Submit 
  loadingText="Loading..."
  className="h-10 w-full text-sm sm:h-11 sm:text-base"
>
  Submit
</FormBuilder.Submit>
```

### Input Styling
```typescript
<FormBuilder.Text
  name="nationalId"
  label="National ID"
  className="text-center tracking-wider"  // Centered with letter spacing
/>
```

## Common Patterns & Best Practices

### ✅ DO

```typescript
// ✅ Use client directive for forms
'use client';

// ✅ Define schemas in separate validation files
import { mySchema, type MyFormValues } from './validations';

// ✅ ALWAYS use shared regex patterns from @/lib/validations/regex
import { ARABIC_TEXT_REGEX, ENGLISH_TEXT_REGEX } from '@/lib/validations/regex';

// ✅ Use shared regex in Zod schemas
nameArabic: z.string().regex(ARABIC_TEXT_REGEX, 'يجب إدخال أحرف عربية فقط')

// ✅ Use shared regex in onChange handlers
if (ARABIC_TEXT_OPTIONAL_REGEX.test(value)) { ... }

// ✅ Export types from Zod schemas
export type LoginFormValues = z.infer<typeof loginFormSchema>;

// ✅ Pass loading state from mutation
loading={mutation.isPending}

// ✅ Use typed submit handlers
const handleSubmit = (values: LoginFormValues) => {
  mutation.mutate(values);
};

// ✅ Format inputs on change by filtering characters
onChange={(e) => {
  const filtered = e.target.value
    .split('')
    .filter((char) => /[a-zA-Z\s-]/.test(char))
    .join('');
  e.target.value = filtered;
}}

// ✅ Use responsive Tailwind classes
className="space-y-3 sm:space-y-4"

// ✅ Extract complex form content to separate components
<FormBuilder {...props}>
  <FormContent />
</FormBuilder>

// ✅ Use useFormContext for conditional rendering
const { watch } = useFormContext();
const value = watch('fieldName');
```

### ❌ DON'T

```typescript
// ❌ Don't forget 'use client' directive
// Forms require client-side interactivity

// ❌ Don't use inline regex patterns
nameArabic: z.string().regex(/^[\u0600-\u06FF\s-]+$/, '...')  // Bad
if (/^[a-zA-Z\s-]*$/.test(value)) { ... }  // Bad

// ✅ Use shared regex instead
nameArabic: z.string().regex(ARABIC_TEXT_REGEX, '...')  // Good
if (ENGLISH_TEXT_OPTIONAL_REGEX.test(value)) { ... }  // Good

// ❌ Don't use 'any' type
const handleSubmit = (values: any) => {}  // Bad

// ❌ Don't define schemas inline
<FormBuilder schema={z.object({...})}>  // Bad

// ❌ Don't access form values from outside FormBuilder
const form = useForm();  // Bad - FormBuilder manages this

// ❌ Don't manually manage loading state
const [isLoading, setIsLoading] = useState(false);  // Bad - use mutation state

// ❌ Don't skip validation schemas
<FormBuilder onSubmit={handleSubmit}>  // Bad - always use schema

// ❌ Don't forget loadingText on Submit
<FormBuilder.Submit>Submit</FormBuilder.Submit>  // Bad - add loadingText
```

## File Organization

```
app/(feature)/
├── components/
│   └── MyForm.tsx              # Form component
├── validations/
│   └── index.ts                # or validations.ts
├── hooks/
│   └── useMutation.ts          # Custom mutation hooks
└── page.tsx                    # Page using the form
```

## Testing Considerations

When writing tests for FormBuilder forms:

1. Mock the mutation hooks
2. Test validation by submitting invalid data
3. Test loading states
4. Test conditional field rendering
5. Test multi-step form navigation

## Performance Tips

1. Use `defaultValues` to initialize forms
2. Extract complex form content to separate components
3. Memoize expensive computed values in form content
4. Use `onChange` for input formatting, not complex logic
5. Keep validation schemas simple and focused

## Common Issues & Solutions

### Issue: Form not submitting
**Solution**: Ensure schema validation passes and check console for errors

### Issue: Loading state not working
**Solution**: Pass `loading={mutation.isPending}` to FormBuilder root

### Issue: Conditional fields not updating
**Solution**: Use `useFormContext` and `watch` inside FormBuilder children

### Issue: Default values not applying
**Solution**: Ensure `defaultValues` object keys match schema field names

### Issue: Type errors on submit
**Solution**: Use exported type from Zod schema: `type MyFormValues = z.infer<typeof mySchema>`
