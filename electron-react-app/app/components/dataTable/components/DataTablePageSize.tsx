import type { Table } from '@tanstack/react-table';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/app/components/ui/select';

const PAGE_SIZE_OPTIONS = [10, 20, 30, 50, 100];

interface DataTablePageSizeProps<TData> {
  table: Table<TData>;
  onPageSizeChange?: (size: number) => void;
}

export function DataTablePageSize<TData>({
  table,
  onPageSizeChange,
}: DataTablePageSizeProps<TData>) {
  return (
    <div className="flex items-center gap-2">
      <span className="text-muted-foreground text-sm">صفوف لكل صفحة</span>
      <Select
        value={String(table.getState().pagination.pageSize)}
        onValueChange={(value) => onPageSizeChange?.(Number(value))}
      >
        <SelectTrigger className="w-[70px]">
          <SelectValue />
        </SelectTrigger>
        <SelectContent>
          {PAGE_SIZE_OPTIONS.map((size) => (
            <SelectItem key={size} value={String(size)}>
              {size}
            </SelectItem>
          ))}
        </SelectContent>
      </Select>
    </div>
  );
}
