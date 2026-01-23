import { MediaCategory } from '@/app/components/ui/image-upload';
import type { LucideIcon } from 'lucide-react';

// Base field props
export interface BaseFieldConfig {
  name: string;
  label?: string;
  placeholder?: string;
  description?: string;
  required?: boolean;
  disabled?: boolean;
  hidden?: boolean;
  icon?: LucideIcon;
}

// Text input field
export interface TextFieldConfig
  extends BaseFieldConfig,
  Omit<
    React.InputHTMLAttributes<HTMLInputElement>,
    'name' | 'type' | 'onChange' | 'value'
  > {
  type: 'text' | 'email' | 'password' | 'textarea' | 'hidden';
  rows?: number;
}

// Number input field
export interface NumberFieldConfig extends BaseFieldConfig {
  type: 'number';
  min?: number;
  max?: number;
  step?: number;
}

// Select field
export interface SelectFieldConfig extends BaseFieldConfig {
  type: 'select';
  options?: { label: string; value: string | number }[];
}

// Phone input field
export interface PhoneFieldConfig extends BaseFieldConfig {
  type: 'phone';
  defaultCountry?: string;
}

// Checkbox field
export interface CheckboxFieldConfig extends BaseFieldConfig {
  type: 'checkbox';
}

// Switch field
export interface SwitchFieldConfig extends BaseFieldConfig {
  type: 'switch';
}

// Radio group field
export interface RadioFieldConfig extends BaseFieldConfig {
  type: 'radio';
  options?: { label: string; value: string | number }[];
}

// Date picker field
export interface DatePickerFieldConfig extends BaseFieldConfig {
  type: 'date-picker';
  minDate?: Date;
  maxDate?: Date;
  dateFormat?: string;
}

// Image upload field
export interface ImageUploadFieldConfig extends BaseFieldConfig {
  type: 'image-upload';
  mediaCategory?: MediaCategory;
  maxSize?: number;
  previewAlt?: string;
  onUpload?: (
    file: File,
    category: MediaCategory,
  ) => Promise<{ success: boolean; data?: { url: string }; error?: string }>;
}

// Union type
export type FormFieldConfig =
  | TextFieldConfig
  | NumberFieldConfig
  | SelectFieldConfig
  | PhoneFieldConfig
  | CheckboxFieldConfig
  | SwitchFieldConfig
  | RadioFieldConfig
  | DatePickerFieldConfig
  | ImageUploadFieldConfig;
