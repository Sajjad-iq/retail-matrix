import {
  FormControl,
  FormDescription,
  FormItem,
  FormLabel,
  FormMessage,
} from '@/app/components/ui/form';
import { ImageUpload, MediaCategory } from '@/app/components/ui/image-upload';
import type { ImageUploadFieldConfig } from '../types';

interface ImageUploadFieldProps {
  field: ImageUploadFieldConfig;
  value: string;
  onChange: (value: string) => void;
  isLoading: boolean;
}

export function ImageUploadField({
  field,
  value,
  onChange,
  isLoading,
}: ImageUploadFieldProps) {
  return (
    <FormItem>
      {field.label && (
        <FormLabel>
          {field.label}
          {field.required && <span className="text-destructive">*</span>}
        </FormLabel>
      )}
      <FormControl>
        <ImageUpload
          value={value}
          onChange={onChange}
          category={field.mediaCategory || MediaCategory.IMAGE}
          maxSize={field.maxSize || 5}
          previewAlt={field.previewAlt || 'Image preview'}
          disabled={isLoading || field.disabled}
          onUpload={field.onUpload}
        />
      </FormControl>
      {field.description && (
        <FormDescription>{field.description}</FormDescription>
      )}
      <FormMessage />
    </FormItem>
  );
}
