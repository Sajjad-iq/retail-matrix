'use client';

import * as React from 'react';
import { CalendarIcon } from 'lucide-react';
import { Button } from '@/app/components/ui/button';
import { Calendar } from '@/app/components/ui/calendar';
import { Input } from '@/app/components/ui/input';
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from '@/app/components/ui/popover';
import {
  FormControl,
  FormDescription,
  FormItem,
  FormLabel,
  FormMessage,
} from '@/app/components/ui/form';
import type { BaseFieldConfig } from '../types';

interface DatePickerFieldProps {
  field: BaseFieldConfig & {
    type: 'date-picker';
    minDate?: Date;
    maxDate?: Date;
    dateFormat?: string;
  };
  value: Date | string | undefined;
  onChange: (value: Date | string | undefined) => void;
  isLoading?: boolean;
}

function formatDate(date: Date | undefined, format?: string) {
  if (!date) {
    return '';
  }

  if (format === 'iso') {
    return date.toISOString().split('T')[0];
  }

  return date.toLocaleDateString('en-US', {
    day: '2-digit',
    month: 'long',
    year: 'numeric',
  });
}

function isValidDate(date: Date | undefined) {
  if (!date) {
    return false;
  }
  return !isNaN(date.getTime());
}

function parseValue(value: Date | string | undefined): Date | undefined {
  if (!value) return undefined;
  if (value instanceof Date) return value;
  const parsed = new Date(value);
  return isValidDate(parsed) ? parsed : undefined;
}

export function DatePickerField({
  field,
  value,
  onChange,
  isLoading,
}: DatePickerFieldProps) {
  const [open, setOpen] = React.useState(false);
  const parsedDate = React.useMemo(() => parseValue(value), [value]);
  const [month, setMonth] = React.useState<Date | undefined>(parsedDate);
  const [inputValue, setInputValue] = React.useState(
    formatDate(parsedDate, field.dateFormat),
  );

  // Update input value when date changes
  React.useEffect(() => {
    setInputValue(formatDate(parsedDate, field.dateFormat));
  }, [parsedDate, field.dateFormat]);

  return (
    <FormItem>
      {field.label && (
        <FormLabel>
          {field.label}
          {field.required && <span className="text-destructive ml-1">*</span>}
        </FormLabel>
      )}
      <FormControl>
        <div className="relative flex gap-2">
          <Input
            value={inputValue}
            placeholder={field.placeholder || 'Select date'}
            className="bg-background pr-10"
            disabled={isLoading || field.disabled}
            onChange={(e) => {
              const date = new Date(e.target.value);
              setInputValue(e.target.value);
              if (isValidDate(date)) {
                // Return string in ISO format if specified, otherwise return Date object
                onChange(
                  field.dateFormat === 'iso' ? formatDate(date, 'iso') : date,
                );
                setMonth(date);
              }
            }}
            onKeyDown={(e) => {
              if (e.key === 'ArrowDown') {
                e.preventDefault();
                setOpen(true);
              }
            }}
          />
          <Popover open={open} onOpenChange={setOpen}>
            <PopoverTrigger asChild>
              <Button
                variant="ghost"
                className="absolute top-1/2 right-2 size-6 -translate-y-1/2"
                disabled={isLoading || field.disabled}
                type="button"
              >
                <CalendarIcon className="size-3.5" />
                <span className="sr-only">Select date</span>
              </Button>
            </PopoverTrigger>
            <PopoverContent
              className="w-auto overflow-hidden p-0"
              align="end"
              alignOffset={-8}
              sideOffset={10}
            >
              <Calendar
                mode="single"
                selected={parsedDate}
                captionLayout="dropdown"
                month={month}
                onMonthChange={setMonth}
                onSelect={(date) => {
                  // Return string in ISO format if specified, otherwise return Date object
                  onChange(
                    field.dateFormat === 'iso' ? formatDate(date, 'iso') : date,
                  );
                  setInputValue(formatDate(date, field.dateFormat));
                  setOpen(false);
                }}
                fromDate={field.minDate}
                toDate={field.maxDate}
                disabled={isLoading || field.disabled}
              />
            </PopoverContent>
          </Popover>
        </div>
      </FormControl>
      {field.description && (
        <FormDescription>{field.description}</FormDescription>
      )}
      <FormMessage />
    </FormItem>
  );
}
