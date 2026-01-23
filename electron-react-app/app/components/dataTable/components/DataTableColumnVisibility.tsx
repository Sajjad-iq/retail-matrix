import type { Table } from '@tanstack/react-table';
import { ChevronDown } from 'lucide-react';
import { Button } from '@/app/components/ui/button';
import {
  DropdownMenu,
  DropdownMenuCheckboxItem,
  DropdownMenuContent,
  DropdownMenuTrigger,
} from '@/app/components/ui/dropdown-menu';

interface DataTableColumnVisibilityProps<TData> {
  table: Table<TData>;
}

/**
 * DataTable Column Visibility Component
 *
 * Dropdown menu to toggle column visibility
 */
export function DataTableColumnVisibility<TData>({
  table,
}: DataTableColumnVisibilityProps<TData>) {
  return (
    <DropdownMenu>
      <DropdownMenuTrigger asChild>
        <Button variant="outline" size="sm" className="text-xs sm:text-sm">
          <span className="hidden sm:inline">الأعمدة</span>
          <span className="sm:hidden">أعمدة</span>
          <ChevronDown className="ms-1 h-4 w-4 sm:ms-2" />
        </Button>
      </DropdownMenuTrigger>
      <DropdownMenuContent align="end">
        {table
          .getAllColumns()
          .filter((column) => column.getCanHide())
          .map((column) => {
            // Get the column header text
            // Priority: meta.label > string header > column.id
            const meta = column.columnDef.meta as
              | { label?: string }
              | undefined;
            const header = column.columnDef.header;

            let headerText = column.id;
            if (meta?.label) {
              headerText = meta.label;
            } else if (typeof header === 'string') {
              headerText = header;
            }

            return (
              <DropdownMenuCheckboxItem
                key={column.id}
                checked={column.getIsVisible()}
                onCheckedChange={(value) => column.toggleVisibility(!!value)}
              >
                {headerText}
              </DropdownMenuCheckboxItem>
            );
          })}
      </DropdownMenuContent>
    </DropdownMenu>
  );
}
