import {
  FormControl,
  FormDescription,
  FormItem,
  FormLabel,
  FormMessage,
} from '@/app/components/ui/form';
import { RadioGroup, RadioGroupItem } from '@/app/components/ui/radio-group';
import type { RadioFieldConfig } from '../types';

interface RadioGroupFieldProps {
  field: RadioFieldConfig;
  value: string;
  onChange: (value: string) => void;
  isLoading: boolean;
}

export function RadioGroupField({
  field,
  value,
  onChange,
  isLoading,
}: RadioGroupFieldProps) {
  return (
    <FormItem className="space-y-3">
      {field.label && (
        <FormLabel>
          {field.label}
          {field.required && <span className="text-destructive">*</span>}
        </FormLabel>
      )}
      <FormControl>
        <RadioGroup
          value={value}
          onValueChange={onChange}
          disabled={isLoading || field.disabled}
          className="flex flex-col space-y-1"
        >
          {(field.options || []).map((option) => (
            <FormItem
              key={option.value}
              className="flex items-center space-y-0 space-x-3"
            >
              <FormControl>
                <RadioGroupItem value={String(option.value)} />
              </FormControl>
              <FormLabel className="cursor-pointer font-normal">
                {option.label}
              </FormLabel>
            </FormItem>
          ))}
        </RadioGroup>
      </FormControl>
      {field.description && (
        <FormDescription>{field.description}</FormDescription>
      )}
      <FormMessage />
    </FormItem>
  );
}
