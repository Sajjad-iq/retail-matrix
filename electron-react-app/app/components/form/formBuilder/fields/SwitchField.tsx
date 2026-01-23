import {
  FormControl,
  FormDescription,
  FormItem,
  FormLabel,
  FormMessage,
} from '@/app/components/ui/form';
import { Switch } from '@/app/components/ui/switch';
import type { FormFieldConfig } from '../types';

interface SwitchFieldProps {
  field: FormFieldConfig;
  value: boolean;
  onChange: (value: boolean) => void;
  isLoading: boolean;
}

export function SwitchField({
  field,
  value,
  onChange,
  isLoading,
}: SwitchFieldProps) {
  return (
    <FormItem className="flex flex-row items-center justify-between rounded-lg border p-4">
      <div className="space-y-0.5">
        {field.label && (
          <FormLabel className="text-base">
            {field.label}
            {field.required && <span className="text-destructive">*</span>}
          </FormLabel>
        )}
        {field.description && (
          <FormDescription>{field.description}</FormDescription>
        )}
      </div>
      <FormControl>
        <Switch
          checked={value}
          onCheckedChange={onChange}
          disabled={isLoading || field.disabled}
        />
      </FormControl>
      <FormMessage />
    </FormItem>
  );
}
