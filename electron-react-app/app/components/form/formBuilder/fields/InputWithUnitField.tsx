'use client';

import {
  FormControl,
  FormDescription,
  FormItem,
  FormLabel,
  FormMessage,
} from '@/app/components/ui/form';
import { Input } from '@/app/components/ui/input';
import {
  Select,
  SelectContent,
  SelectGroup,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/app/components/ui/select';
import type { InputWithUnitFieldConfig } from '../types';

interface InputWithUnitFieldProps {
  field: InputWithUnitFieldConfig;
  value: { [key: string]: any };
  onChange: (value: { [key: string]: any }) => void;
  isLoading: boolean;
}

export function InputWithUnitField({
  field,
  value,
  onChange,
  isLoading,
}: InputWithUnitFieldProps) {
  const {
    valueKey = 'value',
    unitKey = 'unit',
    options = [],
    placeholder,
    unitPlaceholder,
  } = field;

  const currentValue = value?.[valueKey] ?? '';
  const currentUnit = value?.[unitKey] ?? '';

  const handleValueChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const inputValue = e.target.value;
    // Allow empty or convert to number if valid
    const parsedValue = inputValue === '' ? '' : (isNaN(Number(inputValue)) ? inputValue : Number(inputValue));
    onChange({
      ...value,
      [valueKey]: parsedValue,
    });
  };

  const handleUnitChange = (unit: string) => {
    onChange({
      ...value,
      [unitKey]: unit,
    });
  };

  return (
    <FormItem className={field.hidden ? 'hidden' : ''}>
      {field.label && !field.hidden && (
        <FormLabel>
          {field.label}
          {field.required && <span className="text-destructive">*</span>}
        </FormLabel>
      )}
      <FormControl>
        <div className="flex">
          <Input
            type="text"
            value={currentValue}
            onChange={handleValueChange}
            disabled={isLoading || field.disabled}
            placeholder={placeholder}
            className="rounded-l-md rounded-r-none border-l"
          />
          <Select
            value={String(currentUnit)}
            onValueChange={handleUnitChange}
            disabled={isLoading || field.disabled}
          >
            <SelectTrigger className="w-[120px] rounded-r-md rounded-l-none border-r border-l-0">
              <SelectValue placeholder={unitPlaceholder || 'الوحدة'} />
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
        </div>
      </FormControl>
      {field.description && (
        <FormDescription>{field.description}</FormDescription>
      )}
      <FormMessage />
    </FormItem>
  );
}
