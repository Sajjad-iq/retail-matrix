import {
  FormControl,
  FormDescription,
  FormItem,
  FormLabel,
  FormMessage,
} from '@/app/components/ui/form';
import { PhoneInput } from '@/app/components/ui/phone-input';
import type { PhoneFieldConfig } from '../types';

interface PhoneInputFieldProps {
  field: PhoneFieldConfig;
  /** Combined phone value (e.g., "+964 7501234567") */
  value: string;
  /** Called with combined phone number */
  onChange: (value: string) => void;
  isLoading: boolean;
}

export function PhoneInputField({
  field,
  value,
  onChange,
  isLoading,
}: PhoneInputFieldProps) {
  return (
    <FormItem>
      {field.label && (
        <FormLabel>
          {field.label}
          {field.required && <span className="text-destructive">*</span>}
        </FormLabel>
      )}
      <FormControl>
        <PhoneInput
          value={value}
          onChange={onChange}
          disabled={isLoading || field.disabled}
          placeholder={field.placeholder}
          defaultCountry={field.defaultCountry || 'IQ'}
        />
      </FormControl>
      {field.description && (
        <FormDescription>{field.description}</FormDescription>
      )}
      <FormMessage />
    </FormItem>
  );
}
