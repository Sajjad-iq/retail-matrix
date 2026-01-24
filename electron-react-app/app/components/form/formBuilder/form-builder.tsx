'use client';

import * as React from 'react';
import { useForm, useFormContext } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import type z from 'zod';
import { Button } from '@/app/components/ui/button';
import { Form, FormField } from '@/app/components/ui/form';
import { Loader2, type LucideIcon } from 'lucide-react';

import { TextInputField } from './fields/TextInputField';
import { CheckboxField } from './fields/CheckboxField';
import { SwitchField } from './fields/SwitchField';
import { RadioGroupField } from './fields/RadioGroupField';
import { PhoneInputField } from './fields/PhoneInputField';
import { DatePickerField } from './fields/DatePickerField';
import { ImageUploadField } from './fields/ImageUploadField';
import { InputWithUnitField } from './fields/InputWithUnitField';

// Loading context (only thing Form doesn't provide)
const LoadingContext = React.createContext(false);

// Base field props - extends HTML input attributes
interface FieldProps
  extends Omit<
    React.InputHTMLAttributes<HTMLInputElement>,
    'name' | 'type' | 'value'
  > {
  name: string;
  label?: string;
  placeholder?: string;
  description?: string;
  required?: boolean;
  disabled?: boolean;
  icon?: LucideIcon;
}

// Field Components - use react-hook-form's useFormContext
function TextField(props: FieldProps) {
  const { control } = useFormContext();
  const loading = React.useContext(LoadingContext);
  return (
    <FormField
      control={control}
      name={props.name}
      render={({ field }) => (
        <TextInputField
          field={{ ...props, type: 'text' }}
          value={field.value}
          onChange={field.onChange}
          isLoading={loading}
        />
      )}
    />
  );
}

function EmailField(props: FieldProps) {
  const { control } = useFormContext();
  const loading = React.useContext(LoadingContext);
  return (
    <FormField
      control={control}
      name={props.name}
      render={({ field }) => (
        <TextInputField
          field={{ ...props, type: 'email' }}
          value={field.value}
          onChange={field.onChange}
          isLoading={loading}
        />
      )}
    />
  );
}

function PasswordField(props: FieldProps) {
  const { control } = useFormContext();
  const loading = React.useContext(LoadingContext);
  return (
    <FormField
      control={control}
      name={props.name}
      render={({ field }) => (
        <TextInputField
          field={{ ...props, type: 'password' }}
          value={field.value}
          onChange={field.onChange}
          isLoading={loading}
        />
      )}
    />
  );
}

function TextareaField(props: FieldProps & { rows?: number }) {
  const { control } = useFormContext();
  const loading = React.useContext(LoadingContext);
  return (
    <FormField
      control={control}
      name={props.name}
      render={({ field }) => (
        <TextInputField
          field={{ ...props, type: 'textarea' }}
          value={field.value}
          onChange={field.onChange}
          isLoading={loading}
        />
      )}
    />
  );
}

function NumberField(
  props: FieldProps & { min?: number; max?: number; step?: number },
) {
  const { control } = useFormContext();
  const loading = React.useContext(LoadingContext);
  return (
    <FormField
      control={control}
      name={props.name}
      render={({ field }) => (
        <TextInputField
          field={{ ...props, type: 'number' }}
          value={field.value}
          onChange={field.onChange}
          isLoading={loading}
        />
      )}
    />
  );
}

function SelectField(
  props: FieldProps & { options: { label: string; value: string }[] },
) {
  const { control } = useFormContext();
  const loading = React.useContext(LoadingContext);
  return (
    <FormField
      control={control}
      name={props.name}
      render={({ field }) => (
        <TextInputField
          field={{ ...props, type: 'select' }}
          value={field.value}
          onChange={field.onChange}
          isLoading={loading}
        />
      )}
    />
  );
}

function PhoneField(props: FieldProps & { defaultCountry?: string }) {
  const { control } = useFormContext();
  const loading = React.useContext(LoadingContext);
  return (
    <FormField
      control={control}
      name={props.name}
      render={({ field }) => (
        <PhoneInputField
          field={{ ...props, type: 'phone' }}
          value={field.value}
          onChange={field.onChange}
          isLoading={loading}
        />
      )}
    />
  );
}

function FormCheckbox(props: FieldProps) {
  const { control } = useFormContext();
  const loading = React.useContext(LoadingContext);
  return (
    <FormField
      control={control}
      name={props.name}
      render={({ field }) => (
        <CheckboxField
          field={{ ...props, type: 'checkbox' }}
          value={field.value}
          onChange={field.onChange}
          isLoading={loading}
        />
      )}
    />
  );
}

function FormSwitch(props: FieldProps) {
  const { control } = useFormContext();
  const loading = React.useContext(LoadingContext);
  return (
    <FormField
      control={control}
      name={props.name}
      render={({ field }) => (
        <SwitchField
          field={{ ...props, type: 'switch' }}
          value={field.value}
          onChange={field.onChange}
          isLoading={loading}
        />
      )}
    />
  );
}

function RadioField(
  props: FieldProps & { options: { label: string; value: string }[] },
) {
  const { control } = useFormContext();
  const loading = React.useContext(LoadingContext);
  return (
    <FormField
      control={control}
      name={props.name}
      render={({ field }) => (
        <RadioGroupField
          field={{ ...props, type: 'radio' }}
          value={field.value}
          onChange={field.onChange}
          isLoading={loading}
        />
      )}
    />
  );
}

function DateField(
  props: FieldProps & { minDate?: Date; maxDate?: Date; dateFormat?: string },
) {
  const { control } = useFormContext();
  const loading = React.useContext(LoadingContext);
  return (
    <FormField
      control={control}
      name={props.name}
      render={({ field }) => (
        <DatePickerField
          field={{ ...props, type: 'date-picker' }}
          value={field.value}
          onChange={field.onChange}
          isLoading={loading}
        />
      )}
    />
  );
}

function ImageField(
  props: FieldProps & {
    organizationId?: string;
    mediaCategory?: any;
    maxSize?: number;
    previewAlt?: string;
    onUpload?: any;
  },
) {
  const { control } = useFormContext();
  const loading = React.useContext(LoadingContext);
  return (
    <FormField
      control={control}
      name={props.name}
      render={({ field }) => (
        <ImageUploadField
          field={{ ...props, type: 'image-upload' }}
          value={field.value}
          onChange={field.onChange}
          isLoading={loading}
        />
      )}
    />
  );
}

function InputWithUnitInputField(
  props: FieldProps & {
    options: { label: string; value: string | number }[];
    valueKey?: string;
    unitKey?: string;
    unitPlaceholder?: string;
  },
) {
  const { control } = useFormContext();
  const loading = React.useContext(LoadingContext);
  return (
    <FormField
      control={control}
      name={props.name}
      render={({ field }) => (
        <InputWithUnitField
          field={{ ...props, type: 'input-with-unit' }}
          value={field.value}
          onChange={field.onChange}
          isLoading={loading}
        />
      )}
    />
  );
}

function SubmitButton({
  children,
  loadingText,
  className,
}: {
  children?: React.ReactNode;
  loadingText?: string;
  className?: string;
}) {
  const loading = React.useContext(LoadingContext);
  return (
    <Button
      type="submit"
      disabled={loading}
      className={className}
    >
      {loading ? (
        <>
          <Loader2 className="mr-2 h-4 w-4 animate-spin" />
          {loadingText}
        </>
      ) : (
        children
      )}
    </Button>
  );
}

// Main FormBuilder
export interface FormBuilderProps {
  children: React.ReactNode;
  onSubmit: (values: any) => Promise<void> | void;
  schema?: z.ZodType<Record<string, any>>;
  defaultValues?: Record<string, any>;
  loading?: boolean;
  className?: string;
}

function FormBuilderRoot({
  children,
  onSubmit,
  schema,
  defaultValues = {},
  loading = false,
  className = 'space-y-4',
}: FormBuilderProps) {
  const form = useForm({
    resolver: schema ? zodResolver(schema as any) : undefined,
    defaultValues,
  });

  const prev = React.useRef('');
  React.useEffect(() => {
    const str = JSON.stringify(defaultValues);
    if (str !== prev.current) {
      form.reset(defaultValues);
      prev.current = str;
    }
  }, [defaultValues, form]);

  return (
    <LoadingContext.Provider value={loading}>
      <Form {...form}>
        <form onSubmit={form.handleSubmit(onSubmit)} className={className}>
          {children}
        </form>
      </Form>
    </LoadingContext.Provider>
  );
}

export const FormBuilder = Object.assign(FormBuilderRoot, {
  Text: TextField,
  Email: EmailField,
  Password: PasswordField,
  Textarea: TextareaField,
  Number: NumberField,
  Select: SelectField,
  Phone: PhoneField,
  Checkbox: FormCheckbox,
  Switch: FormSwitch,
  Radio: RadioField,
  Date: DateField,
  Image: ImageField,
  InputWithUnit: InputWithUnitInputField,
  Submit: SubmitButton,
});
