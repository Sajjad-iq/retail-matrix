# FormBuilder Real-World Examples

> Examples extracted from the login components in `app/(identity)/login/components/`
> 
> **📖 For validation schemas and patterns, see [validation-patterns.mdc](./validation-patterns.mdc)**

## Example 1: Simple Login Form
**Source**: `NormalLoginForm.tsx`

```typescript
'use client';

import { FormBuilder } from '@/components/form';
import {
  loginFormSchema,
  type LoginFormValues,
} from '@/app/(identity)/login/validations';
import { useLogin } from '@/app/(identity)/login/hooks';

export function NormalLoginForm({
  isVisible,
  onSwitchToInsured,
}: NormalLoginFormProps) {
  const loginMutation = useLogin();

  const handleSubmit = (values: LoginFormValues) => {
    loginMutation.mutate(values);
  };

  return (
    <Card>
      <CardContent>
        <FormBuilder
          onSubmit={handleSubmit}
          schema={loginFormSchema}
          defaultValues={{ emailOrPhone: '', password: '' }}
          loading={loginMutation.isPending}
          className="space-y-3 sm:space-y-4"
        >
          <FormBuilder.Text
            name="emailOrPhone"
            label="البريد الالكتروني أو رقم الهاتف"
            placeholder="you@example.com أو 07XXXXXXXXX"
            required
          />

          <FormBuilder.Password
            name="password"
            label="كلمة المرور"
            placeholder="••••••••"
            required
          />

          <FormBuilder.Submit
            loadingText="يتم تسجيل الدخول..."
            className="h-10 w-full text-sm sm:h-11 sm:text-base"
          >
            تسجيل الدخول
          </FormBuilder.Submit>

          <div className="text-muted-foreground text-center text-xs sm:text-sm">
            هل نسيت كلمة السر؟{' '}
            <Link href="/forgot-password" className="underline">
              تعيين كلمة مرور جديدة
            </Link>
          </div>

          {/* Additional button outside FormBuilder.Submit */}
          <Button
            type="button"
            onClick={onSwitchToInsured}
          >
            تسجيل دخول المضمونين
          </Button>
        </FormBuilder>
      </CardContent>
    </Card>
  );
}
```

**Validation Schema**:
```typescript
// validations.ts
export const loginFormSchema = z.object({
  emailOrPhone: z
    .string()
    .min(1, 'البريد الالكتروني أو رقم الهاتف مطلوب')
    .superRefine((value, ctx) => {
      if (PHONE_NUMBER_PATTERN.test(value)) {
        if (!PHONE_REGEX.test(value)) {
          ctx.addIssue({
            code: 'custom',
            message: 'رقم الهاتف يجب أن يبدأ بـ 07 ويتكون من 11 رقم',
          });
        }
      } else {
        const emailResult = z.email().safeParse(value);
        if (!emailResult.success) {
          ctx.addIssue({
            code: 'custom',
            message: 'البريد الالكتروني غير صحيح',
          });
        }
      }
    }),
  password: z
    .string()
    .regex(PASSWORD_MIN_LENGTH_REGEX, 'كلمة المرور يجب أن تكون 8 أحرف على الأقل')
    .max(100, 'كلمة المرور يجب ألا تتجاوز 100 حرف'),
});

export type LoginFormValues = z.infer<typeof loginFormSchema>;
```

**Key Patterns**:
- ✅ Client component with `'use client'`
- ✅ Separate validation file
- ✅ Custom hook for mutation (`useLogin`)
- ✅ Loading state from mutation (`loginMutation.isPending`)
- ✅ Responsive classes (`sm:` prefix)
- ✅ Typed submit handler
- ✅ Mix of FormBuilder.Submit and regular Button

---

## Example 2: Conditional Fields with useFormContext
**Source**: `InsuredLoginForm/CredentialsStep.tsx`

```typescript
'use client';

import { useFormContext } from 'react-hook-form';
import { FormBuilder } from '@/components/form';
import {
  insuredCredentialsSchema,
  type InsuredCredentialsValues,
} from '@/app/(identity)/login/validations';

// Separate component for form content to use useFormContext
function CredentialsFormContent({ onBack }: { onBack: () => void }) {
  const { watch } = useFormContext<InsuredCredentialsValues>();
  const hasNationalId = watch('hasNationalId');

  return (
    <>
      <FormBuilder.Checkbox 
        name="hasNationalId" 
        label="هل لديك بطاقة موحدة" 
      />

      {hasNationalId ? (
        <div className="space-y-2">
          <FormBuilder.Text
            name="nationalId"
            label="الرقم الوطني (12 رقم)"
            placeholder="123456789012"
            required
            className="text-center tracking-wider"
            onChange={(e) => {
              const value = e.target.value.replace(/[^0-9]/g, '').slice(0, 12);
              e.target.value = value;
            }}
          />
          {watch('nationalId') && (
            <p className="text-muted-foreground text-center text-xs">
              {watch('nationalId')?.length || 0}/12 رقم
            </p>
          )}
        </div>
      ) : (
        <FormBuilder.Text
          name="civilId"
          label="رقم هوية الاحوال المدنية"
          placeholder="أدخل رقم هوية الاحوال المدنية"
          required
          className="text-center tracking-wider"
        />
      )}

      <FormBuilder.Text
        name="employmentNumberOrInsuranceId"
        label="رقم بطاقة الضمان"
        placeholder="رقم بطاقة الضمان (مثال:4567-00000001-123)"
        required
        className="text-center tracking-wider"
        onChange={(e) => {
          e.target.value = formatInsuranceCardId(e.target.value);
        }}
      />

      <FormBuilder.Submit loadingText="جاري التحقق..." className="w-full">
        التالي
      </FormBuilder.Submit>

      <Button
        type="button"
        variant="outline"
        className="w-full"
        onClick={onBack}
      >
        العودة إلى تسجيل الدخول العادي
      </Button>
    </>
  );
}

export default function CredentialsStep({
  onSuccess,
  onBack,
}: CredentialsStepProps) {
  const handleSubmit = async (values: InsuredCredentialsValues) => {
    const idType = values.hasNationalId ? 'NationalID' : 'CivilID';
    const idNumber = values.hasNationalId
      ? values.nationalId!
      : values.civilId!;

    onSuccess({
      idType,
      idNumber,
      insuranceIDNumber: values.employmentNumberOrInsuranceId,
    });
  };

  return (
    <FormBuilder
      onSubmit={handleSubmit}
      schema={insuredCredentialsSchema}
      defaultValues={{
        hasNationalId: true,
        nationalId: '',
        civilId: '',
        employmentNumberOrInsuranceId: '',
      }}
      className="space-y-3 sm:space-y-4"
    >
      <CredentialsFormContent onBack={onBack} />
    </FormBuilder>
  );
}
```

**Validation Schema**:
```typescript
export const insuredCredentialsSchema = z
  .object({
    hasNationalId: z.boolean(),
    nationalId: z.string().max(NATIONAL_ID_LENGTH).optional(),
    civilId: z.string().optional(),
    employmentNumberOrInsuranceId: z
      .string()
      .min(1, 'رقم بطاقة الضمان مطلوب')
      .regex(
        INSURANCE_CARD_ID_REGEX,
        'رقم بطاقة الضمان يجب أن يكون بالصيغة: (XXX-XXXXXXXX-XXXX)',
      ),
  })
  .superRefine((data, ctx) => {
    if (data.hasNationalId) {
      if (!data.nationalId || data.nationalId.length !== NATIONAL_ID_LENGTH) {
        ctx.addIssue({
          code: 'custom',
          message: 'الرقم الوطني يجب أن يتكون من 12 رقم',
          path: ['nationalId'],
        });
      }
    } else {
      if (!data.civilId || data.civilId.length === 0) {
        ctx.addIssue({
          code: 'custom',
          message: 'رقم هوية الاحوال المدنية مطلوب',
          path: ['civilId'],
        });
      }
    }
  });

export type InsuredCredentialsValues = z.infer<typeof insuredCredentialsSchema>;
```

**Key Patterns**:
- ✅ Conditional rendering based on checkbox
- ✅ useFormContext to access form state
- ✅ watch() to monitor field values
- ✅ Dynamic character counter
- ✅ Input formatting on change (digits only, max length)
- ✅ Custom validation helper (formatInsuranceCardId)
- ✅ Separate FormContent component pattern
- ✅ Async submit handler
- ✅ Pass callbacks to parent (onSuccess)

---

## Example 3: Phone Input with Custom Formatting
**Source**: `InsuredLoginForm/PhoneStep.tsx`

```typescript
'use client';

import { FormBuilder } from '@/components/form';
import {
  insuredPhoneSchema,
  type InsuredPhoneValues,
} from '@/app/(identity)/login/validations';
import { useInsuredVerifyCredentialsAndSendOtp } from '@/app/(identity)/login/hooks';

export default function PhoneStep({
  credentials,
  onSuccess,
  onBack,
}: PhoneStepProps) {
  const verifyAndSendOtpMutation = useInsuredVerifyCredentialsAndSendOtp();

  const handleSubmit = async (values: InsuredPhoneValues) => {
    const result = await verifyAndSendOtpMutation.mutateAsync({
      idType: credentials.idType,
      idNumber: credentials.idNumber,
      insuranceIDNumber: credentials.insuranceIDNumber,
      phoneNumber: values.phoneNumber,
    });

    if (result) {
      onSuccess({
        phoneNumber: values.phoneNumber,
        insuredId: result.insuredId,
        fullName: result.fullName,
      });
    }
  };

  return (
    <>
      <div className="mb-6 text-center">
        <h3 className="text-lg font-semibold text-gray-900">
          التحقق من رقم الهاتف
        </h3>
        <p className="mt-1 text-sm text-gray-500">
          سنرسل لك رمز تحقق عبر واتساب
        </p>
      </div>

      <FormBuilder
        onSubmit={handleSubmit}
        schema={insuredPhoneSchema}
        defaultValues={{ phoneNumber: '' }}
        loading={verifyAndSendOtpMutation.isPending}
        className="space-y-4"
      >
        <FormBuilder.Text
          name="phoneNumber"
          label="رقم الهاتف"
          placeholder="07XXXXXXXXX"
          className="text-center tracking-wider"
          required
          onChange={(e) => {
            const value = e.target.value.replace(/[^0-9]/g, '').slice(0, 11);
            e.target.value = value;
          }}
        />

        <div className="flex gap-3">
          <Button
            type="button"
            variant="outline"
            className="flex-1"
            onClick={onBack}
          >
            رجوع
          </Button>
          <FormBuilder.Submit loadingText="جاري الإرسال..." className="flex-1">
            إرسال رمز التحقق
          </FormBuilder.Submit>
        </div>
      </FormBuilder>
    </>
  );
}
```

**Validation Schema**:
```typescript
export const insuredPhoneSchema = z.object({
  phoneNumber: z
    .string()
    .regex(
      PHONE_REGEX,
      'الرجاء إدخال رقم هاتف صحيح (يبدأ بـ 07 ويتكون من 11 رقم)',
    ),
});

export type InsuredPhoneValues = z.infer<typeof insuredPhoneSchema>;
```

**Key Patterns**:
- ✅ mutateAsync for promise-based mutation
- ✅ Conditional navigation based on result
- ✅ Phone input with formatting (digits only, max 11)
- ✅ Button group (Back + Submit)
- ✅ Centered input with tracking
- ✅ Additional context passed to mutation
- ✅ Header section outside FormBuilder

---

## Example 4: Password Form with Strength Indicator
**Source**: `InsuredLoginForm/PasswordStep.tsx`

```typescript
'use client';

import { useState } from 'react';
import { useFormContext } from 'react-hook-form';
import { Label } from '@/components/ui/label';
import { FormBuilder } from '@/components/form';
import {
  insuredPasswordSchema,
  type InsuredPasswordValues,
} from '@/app/(identity)/login/validations';
import { useInsuredSetPassword } from '@/app/(identity)/login/hooks';
import { PasswordStrengthIndicator } from '@/components/ui/password-strength-indicator';

function PasswordFormContent({ onBack }: { onBack: () => void }) {
  const { watch } = useFormContext<InsuredPasswordValues>();
  const password = watch('password');

  return (
    <>
      <div className="space-y-2">
        <Label htmlFor="password">كلمة المرور</Label>
        <div className="relative">
          <FormBuilder.Password
            name="password"
            placeholder="أدخل كلمة المرور"
            required
          />
        </div>

        <PasswordStrengthIndicator password={password || ''} />
      </div>

      <FormBuilder.Password
        name="confirmPassword"
        label="تأكيد كلمة المرور"
        placeholder="أعد إدخال كلمة المرور"
        required
      />

      <div className="flex gap-3">
        <Button
          type="button"
          variant="outline"
          className="flex-1"
          onClick={onBack}
        >
          رجوع
        </Button>
        <FormBuilder.Submit
          loadingText="جاري إكمال التسجيل..."
          className="flex-1"
        >
          إكمال التسجيل
        </FormBuilder.Submit>
      </div>
    </>
  );
}

export default function PasswordStep({
  userId,
  verificationToken,
  onBack,
}: PasswordStepProps) {
  const setPasswordMutation = useInsuredSetPassword();

  const handleSubmit = async (values: InsuredPasswordValues) => {
    await setPasswordMutation.mutateAsync({
      userId,
      password: values.password,
      verificationToken,
    });
  };

  return (
    <>
      <div className="mb-6 text-center">
        <h3 className="text-lg font-semibold text-gray-900">
          إنشاء كلمة مرور
        </h3>
        <p className="mt-1 text-sm text-gray-500">
          الرجاء إنشاء كلمة مرور قوية لحسابك
        </p>
      </div>

      <FormBuilder
        onSubmit={handleSubmit}
        schema={insuredPasswordSchema}
        defaultValues={{ password: '', confirmPassword: '' }}
        loading={setPasswordMutation.isPending}
        className="space-y-4"
      >
        <PasswordFormContent onBack={onBack} />
      </FormBuilder>
    </>
  );
}
```

**Validation Schema**:
```typescript
export const insuredPasswordSchema = z
  .object({
    password: z
      .string()
      .regex(PASSWORD_MIN_LENGTH_REGEX, 'كلمة المرور يجب أن تكون 8 أحرف على الأقل')
      .regex(PASSWORD_UPPERCASE_REGEX, 'يجب أن تحتوي على حرف كبير واحد على الأقل')
      .regex(PASSWORD_LOWERCASE_REGEX, 'يجب أن تحتوي على حرف صغير واحد على الأقل')
      .regex(PASSWORD_SPECIAL_CHAR_REGEX, 'يجب أن تحتوي على رمز خاص واحد على الأقل'),
    confirmPassword: z.string().min(1, 'تأكيد كلمة المرور مطلوب'),
  })
  .refine((data) => data.password === data.confirmPassword, {
    message: 'كلمة المرور غير متطابقة',
    path: ['confirmPassword'],
  });

export type InsuredPasswordValues = z.infer<typeof insuredPasswordSchema>;
```

**Key Patterns**:
- ✅ Password confirmation with .refine()
- ✅ Multiple regex validations
- ✅ Custom password strength indicator
- ✅ watch() to pass current password value
- ✅ Manual Label component usage
- ✅ Additional metadata passed to mutation

---

## Common Patterns Summary

### 1. Form Component Structure
```
FormWrapper (Client Component)
├── Header/Title (Optional, outside FormBuilder)
├── FormBuilder
│   ├── FormContent (Uses useFormContext)
│   │   ├── Fields
│   │   └── FormBuilder.Submit
│   └── Additional Buttons (type="button")
```

### 2. Separation of Concerns
- **Validations**: Separate file with schemas and types
- **Hooks**: Custom mutation hooks
- **Components**: Presentational form components
- **Logic**: Submit handlers with callbacks

### 3. Input Formatting Pattern
```typescript
onChange={(e) => {
  // Format the value
  const formatted = formatValue(e.target.value);
  // Update the input
  e.target.value = formatted;
}}
```

### 4. Multi-Step Form State Management
```typescript
const [step, setStep] = useState(1);
const [data, setData] = useState({});

const handleStep1 = (values) => {
  setData(prev => ({ ...prev, ...values }));
  setStep(2);
};
```

### 5. Conditional Fields Pattern
```typescript
function FormContent() {
  const { watch } = useFormContext<FormValues>();
  const condition = watch('fieldName');
  
  return (
    <>
      <FormBuilder.Checkbox name="fieldName" label="..." />
      {condition ? <Field1 /> : <Field2 />}
    </>
  );
}
```

### 6. Button Groups
```typescript
<div className="flex gap-3">
  <Button type="button" variant="outline" className="flex-1" onClick={onBack}>
    Back
  </Button>
  <FormBuilder.Submit loadingText="..." className="flex-1">
    Next
  </FormBuilder.Submit>
</div>
```

### 7. Character Counter
```typescript
{watch('field') && (
  <p className="text-muted-foreground text-xs text-center">
    {watch('field')?.length || 0}/12 characters
  </p>
)}
```
