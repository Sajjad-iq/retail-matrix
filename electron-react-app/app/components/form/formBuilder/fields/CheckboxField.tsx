import {
  FormControl,
  FormDescription,
  FormItem,
  FormLabel,
  FormMessage,
} from '@/app/components/ui/form';
import { Checkbox } from '@/app/components/ui/checkbox';
import type { FormFieldConfig } from '../types';

interface CheckboxFieldProps {
  field: FormFieldConfig;
  value: boolean;
  onChange: (value: boolean) => void;
  isLoading: boolean;
}

export function CheckboxField({
  field,
  value,
  onChange,
  isLoading,
}: CheckboxFieldProps) {
  return (
    <FormItem className="flex flex-row items-start space-y-0 space-x-3">
      <FormControl>
        <Checkbox
          checked={value}
          onCheckedChange={onChange}
          disabled={isLoading || field.disabled}
        />
      </FormControl>
      <div className="space-y-1 leading-none">
        {field.label && (
          <FormLabel>
            {field.label}
            {field.required && <span className="text-destructive">*</span>}
          </FormLabel>
        )}
        {field.description && (
          <FormDescription>{field.description}</FormDescription>
        )}
      </div>
      <FormMessage />
    </FormItem>
  );
}
