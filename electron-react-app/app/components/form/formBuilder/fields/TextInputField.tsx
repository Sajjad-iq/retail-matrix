'use client';

import { useState } from 'react';
import { Eye, EyeOff } from 'lucide-react';
import {
  FormControl,
  FormDescription,
  FormItem,
  FormLabel,
  FormMessage,
} from '@/app/components/ui/form';
import { Input } from '@/app/components/ui/input';
import { Textarea } from '@/app/components/ui/textarea';
import {
  Select,
  SelectContent,
  SelectGroup,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/app/components/ui/select';
import type { FormFieldConfig } from '../types';

interface TextInputFieldProps {
  field: FormFieldConfig;
  value: any;
  onChange: (value: any) => void;
  isLoading: boolean;
  customInput?: React.ReactNode;
}

export function TextInputField({
  field,
  value,
  onChange,
  isLoading,
  customInput,
}: TextInputFieldProps) {
  const [showPassword, setShowPassword] = useState(false);
  const Icon = field.icon;
  const isPassword = field.type === 'password';

  const renderInput = () => {
    if (customInput) return customInput;

    // Separate FormBuilder-specific props from HTML input props
    const {
      name,
      label,
      description,
      icon,
      type,
      rows,
      options,
      required,
      onChange: customOnChange,
      ...inputProps
    } = field as any;

    // Merge custom onChange with form onChange
    const handleChange = (
      e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>,
    ) => {
      if (customOnChange) {
        customOnChange(e);
      }
      onChange(e.target.value);
    };

    if (field.type === 'textarea') {
      return (
        <Textarea
          value={value}
          onChange={handleChange}
          rows={field.rows || 3}
          {...inputProps}
        />
      );
    }

    if (field.type === 'select') {
      return (
        <Select value={value} onValueChange={onChange}>
          <SelectTrigger disabled={isLoading || field.disabled}>
            <SelectValue
              placeholder={field.placeholder || 'Select an option'}
            />
          </SelectTrigger>
          <SelectContent>
            <SelectGroup>
              {(field.options || []).map((option) => (
                <SelectItem key={option.value} value={String(option.value)}>
                  {option.label}
                </SelectItem>
              ))}
            </SelectGroup>
          </SelectContent>
        </Select>
      );
    }

    if (field.type === 'number') {
      return (
        <Input
          value={value}
          onChange={handleChange}
          type="number"
          className={Icon ? 'pr-10' : ''}
          {...inputProps}
        />
      );
    }

    if (isPassword) {
      return (
        <Input
          value={value}
          onChange={handleChange}
          type={showPassword ? 'text' : 'password'}
          className="pr-10"
          {...inputProps}
        />
      );
    }

    return (
      <Input
        value={value}
        onChange={handleChange}
        type={field.type}
        className={Icon ? 'pr-10' : ''}
        {...inputProps}
      />
    );
  };

  const renderPasswordToggle = () => (
    <button
      type="button"
      onClick={() => setShowPassword((v) => !v)}
      className="absolute top-1/2 right-3 -translate-y-1/2 text-gray-500 transition-colors hover:text-gray-700"
      tabIndex={-1}
    >
      {showPassword ? (
        <EyeOff className="h-4 w-4 sm:h-5 sm:w-5" />
      ) : (
        <Eye className="h-4 w-4 sm:h-5 sm:w-5" />
      )}
    </button>
  );

  return (
    <FormItem className={field.hidden ? 'hidden' : ''}>
      {field.label && !field.hidden && (
        <FormLabel>
          {field.label}
          {field.required && <span className="text-destructive">*</span>}
        </FormLabel>
      )}
      <FormControl>
        {isPassword ? (
          <div className="relative">
            {renderInput()}
            {renderPasswordToggle()}
          </div>
        ) : Icon && field.type !== 'textarea' && field.type !== 'select' ? (
          <div className="relative">
            {renderInput()}
            <Icon className="text-muted-foreground pointer-events-none absolute top-1/2 right-3 h-4 w-4 -translate-y-1/2" />
          </div>
        ) : (
          renderInput()
        )}
      </FormControl>
      {field.description && (
        <FormDescription>{field.description}</FormDescription>
      )}
      <FormMessage />
    </FormItem>
  );
}
