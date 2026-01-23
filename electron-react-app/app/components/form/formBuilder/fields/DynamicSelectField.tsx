import {
  FormControl,
  FormDescription,
  FormItem,
  FormLabel,
  FormMessage,
} from '@/app/components/ui/form';
import {
  Select,
  SelectContent,
  SelectGroup,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/app/components/ui/select';
import type { FormFieldConfig } from '../types';

interface DynamicSelectFieldProps {
  field: FormFieldConfig;
  value: string;
  onChange: (value: string) => void;
  isLoading: boolean;
  options: { label: string; value: string | number }[];
  loadingOptions: boolean;
}

export function DynamicSelectField({
  field,
  value,
  onChange,
  isLoading,
  options,
  loadingOptions,
}: DynamicSelectFieldProps) {
  return (
    <FormItem>
      {field.label && (
        <FormLabel>
          {field.label}
          {field.required && <span className="text-destructive">*</span>}
        </FormLabel>
      )}
      <FormControl>
        <Select
          value={value}
          onValueChange={onChange}
          disabled={isLoading || field.disabled || loadingOptions}
        >
          <SelectTrigger>
            <SelectValue
              placeholder={field.placeholder || 'Select an option'}
            />
          </SelectTrigger>
          <SelectContent>
            <SelectGroup>
              {options.map((option) => (
                <SelectItem key={option.value} value={String(option.value)}>
                  {option.label}
                </SelectItem>
              ))}
            </SelectGroup>
          </SelectContent>
        </Select>
      </FormControl>
      {field.description && (
        <FormDescription>{field.description}</FormDescription>
      )}
      <FormMessage />
    </FormItem>
  );
}
